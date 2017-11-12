using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;
using System.Threading;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using StoreSample.Queries.Settings;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using Newtonsoft.Json;
using System.Text;
using System.Collections.Concurrent;
using EventStore.ClientAPI.Projections;
using System.Net;

namespace StoreSample.Queries.HostedServices
{
    public class EventsSubscriber : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IEventStoreConnection _connection;
        private readonly EventStoreSettings _eventStoreSettings;
        private readonly ConcurrentBag<EventStorePersistentSubscriptionBase> _subscriptions = new ConcurrentBag<EventStorePersistentSubscriptionBase>();

        public EventsSubscriber(IServiceProvider serviceProvider, IEventStoreConnection connection, IOptions<EventStoreSettings> eventStoreOptions)
        {
            _serviceProvider = serviceProvider;
            _connection = connection;
            _eventStoreSettings = eventStoreOptions.Value;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            const string productStream = "$ce-product";
            const string categoryStream = "$ce-category";
            const string queryGroup = "query";

            // Doesn't work for some reasons, have to turn on projections via UI now.
            await EnableProjections();

            await Task.WhenAll(Subscribe(productStream, queryGroup), Subscribe(categoryStream, queryGroup));
        }

        private async Task EnableProjections()
        {
            var hostStartIndex = _eventStoreSettings.ConnectionString.LastIndexOf('@');
            var hostEndIndex = _eventStoreSettings.ConnectionString.LastIndexOf(':');
            var host = _eventStoreSettings.ConnectionString.Substring(hostStartIndex + 1, hostEndIndex - hostStartIndex - 1);
            var port = int.Parse(_eventStoreSettings.ConnectionString.Substring(hostEndIndex + 1, _eventStoreSettings.ConnectionString.Length - hostEndIndex - 1));
            var pm = new ProjectionsManager(_connection.Settings.Log, new DnsEndPoint(host, port), TimeSpan.FromSeconds(10));
            var user = new UserCredentials(_eventStoreSettings.UserName, _eventStoreSettings.Password);

            try
            {
                await Task.WhenAll(pm.EnableAsync("$by_category"), pm.EnableAsync("$by_event_type"), pm.EnableAsync("$stream_by_category"), pm.EnableAsync("$streams"));
            }
            catch (Exception e)
            {
            }
        }

        private async Task Subscribe(string productStream, string queryGroup)
        {
            try
            {
                await _connection.CreatePersistentSubscriptionAsync(productStream, queryGroup, PersistentSubscriptionSettings.Create().ResolveLinkTos().StartFromBeginning(), new UserCredentials(_eventStoreSettings.UserName, _eventStoreSettings.Password));
            }
            catch (Exception e)
            {
            }

            async Task EventAppeared(EventStorePersistentSubscriptionBase s, ResolvedEvent e)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                    var @event = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(e.Event.Data), new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All }) as INotification;
                    await mediator.Publish(@event);
                }
            };

            void SubscriptionDropped(EventStorePersistentSubscriptionBase e, SubscriptionDropReason s, Exception exc)
            {
                _subscriptions.TryTake(out e);
                _subscriptions.Add(_connection.ConnectToPersistentSubscriptionAsync(productStream, queryGroup, EventAppeared, SubscriptionDropped).Result);
            };

            _subscriptions.Add(await _connection.ConnectToPersistentSubscriptionAsync(productStream, queryGroup, EventAppeared, SubscriptionDropped));
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            foreach (var s in _subscriptions)
            {
                s.Stop(TimeSpan.FromMilliseconds(1));
            }

            return Task.CompletedTask;
        }
    }
}
