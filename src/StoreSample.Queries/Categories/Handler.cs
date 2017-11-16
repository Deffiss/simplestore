using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using StoreSample.Core.Categories;
using StoreSample.Queries.Settings;
using System;
using System.Threading.Tasks;

namespace StoreSample.Queries.Categories
{
    public class Handler :
        IAsyncRequestHandler<GetAll, Category[]>,
        IAsyncRequestHandler<GetById, Category>,
        IAsyncNotificationHandler<Created>,
        IAsyncNotificationHandler<Renamed>,
        IAsyncNotificationHandler<PropertiesChanged>
    {
        private readonly MongoClient _mongoClient;
        private readonly MongoSettings _settings;

        public Handler(MongoClient mongoClient, IOptions<MongoSettings> settingsOptions)
        {
            _mongoClient = mongoClient;
            _settings = settingsOptions.Value;
        }


        public async Task<Category[]> Handle(GetAll message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var collection = _mongoClient.GetDatabase(_settings.Database).GetCollection<Category>(nameof(Category));
            return (await collection.Find(Builders<Category>.Filter.Empty).ToListAsync()).ToArray();
        }

        public async Task<Category> Handle(GetById message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var collection = _mongoClient.GetDatabase(_settings.Database).GetCollection<Category>(nameof(Category));
            return await collection.Find(Builders<Category>.Filter.Empty).FirstOrDefaultAsync();
        }

        public async Task Handle(Created notification)
        {
            if (notification == null)
            {
                throw new ArgumentNullException(nameof(notification));
            }

            var collection = _mongoClient.GetDatabase(_settings.Database).GetCollection<Category>(nameof(Category));
            await collection.InsertOneAsync(new Category
            {
                OriginalId = notification.SourceId,
                Name = notification.Name,
                Properties = notification.Properties
            });
        }

        public async Task Handle(Renamed notification)
        {
            if (notification == null)
            {
                throw new ArgumentNullException(nameof(notification));
            }

            var collection = _mongoClient.GetDatabase(_settings.Database).GetCollection<Category>(nameof(Category));
            await collection.UpdateOneAsync(Builders<Category>.Filter.Eq(p => p.OriginalId, notification.SourceId), Builders<Category>.Update.Set(p => p.Name, notification.Name));
        }

        public async Task Handle(PropertiesChanged notification)
        {
            if (notification == null)
            {
                throw new ArgumentNullException(nameof(notification));
            }

            var collection = _mongoClient.GetDatabase(_settings.Database).GetCollection<Category>(nameof(Category));
            await collection.UpdateOneAsync(Builders<Category>.Filter.Eq(p => p.OriginalId, notification.SourceId), Builders<Category>.Update.Set(p => p.Properties, notification.Properties));
        }
    }
}
