using EventStore.ClientAPI;
using MediatR;
using Newtonsoft.Json;
using StoreSample.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreSample.Commands.Repositories
{
    public class EventSourcedRepository<T> : IRepository<T>
        where T : class, IEventSourced
    {
        private readonly IEventStoreConnection _connection;
        private readonly IMediator _mediator;

        public EventSourcedRepository(IEventStoreConnection connection, IMediator mediator)
        {
            _connection = connection;
            _mediator = mediator;
        }

        public async Task<T> Get(Guid id)
        {
            var streamEvents = new List<ResolvedEvent>();

            StreamEventsSlice currentSlice;
            var streamName = GetStreamName(id);
            var nextSliceStart = (long)StreamPosition.Start;
            do
            {
                currentSlice = await _connection.ReadStreamEventsForwardAsync(streamName, nextSliceStart, 200, false);

                nextSliceStart = currentSlice.NextEventNumber;

                streamEvents.AddRange(currentSlice.Events);
            } while (!currentSlice.IsEndOfStream);

            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            var events = streamEvents.Select(e => JsonConvert.DeserializeObject(Encoding.UTF8.GetString(e.Event.Data), settings) as IVersionedEvent);
            var entity = typeof(T).GetConstructor(new[] { typeof(Guid), typeof(IEnumerable<IVersionedEvent>) }).Invoke(new object[] { id, events });

            return (T)entity;
        }

        public async Task Save(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            var events = entity.Events
                .Select(e => new EventData(Guid.NewGuid(), $"{typeof(T).Name.ToLower()}-{e.GetType().Name.ToLower()}", true, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(e, settings)), Array.Empty<byte>()))
                .ToArray();

            await _connection.AppendToStreamAsync(GetStreamName(entity.Id), ExpectedVersion.Any, events);

            foreach (var e in entity.Events)
            {
                await _mediator.Publish(e);
            }
        }

        private static string GetStreamName(Guid id) => $"{typeof(T).Name.ToLower()}-{id.ToString("N").ToLower()}";
    }
}
