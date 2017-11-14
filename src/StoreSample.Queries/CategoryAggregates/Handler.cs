using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using StoreSample.Core.Categories;
using StoreSample.Queries.Settings;
using System;
using System.Threading.Tasks;

namespace StoreSample.Queries.CategoryAggregates
{
    public class Handler :
        IAsyncRequestHandler<GetAll, CategoryAggregate[]>,
        IAsyncNotificationHandler<Created>,
        IAsyncNotificationHandler<ProductAdded>,
        IAsyncNotificationHandler<ProductRemoved>
    {
        private readonly MongoClient _mongoClient;
        private readonly MongoSettings _settings;

        public Handler(MongoClient mongoClient, IOptions<MongoSettings> settingsOptions)
        {
            _mongoClient = mongoClient;
            _settings = settingsOptions.Value;
        }


        public async Task<CategoryAggregate[]> Handle(GetAll message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var collection = _mongoClient.GetDatabase(_settings.Database).GetCollection<CategoryAggregate>(nameof(CategoryAggregate));
            return (await collection.Find(Builders<CategoryAggregate>.Filter.Empty).ToListAsync()).ToArray();
        }

        public async Task Handle(Created notification)
        {
            if (notification == null)
            {
                throw new ArgumentNullException(nameof(notification));
            }

            var collection = _mongoClient.GetDatabase(_settings.Database).GetCollection<CategoryAggregate>(nameof(CategoryAggregate));
            await collection.InsertOneAsync(new CategoryAggregate
            {
                OriginalId = notification.SourceId,
                Name = notification.Name,
                ProductCount = 0
            });
        }

        public async Task Handle(Renamed notification)
        {
            if (notification == null)
            {
                throw new ArgumentNullException(nameof(notification));
            }

            var collection = _mongoClient.GetDatabase(_settings.Database).GetCollection<CategoryAggregate>(nameof(CategoryAggregate));
            await collection.UpdateOneAsync(Builders<CategoryAggregate>.Filter.Eq(p => p.OriginalId, notification.SourceId), Builders<CategoryAggregate>.Update.Set(p => p.Name, notification.Name));
        }

        public async Task Handle(ProductAdded notification)
        {
            if (notification == null)
            {
                throw new ArgumentNullException(nameof(notification));
            }

            var collection = _mongoClient.GetDatabase(_settings.Database).GetCollection<CategoryAggregate>(nameof(CategoryAggregate));
            await collection.UpdateOneAsync(Builders<CategoryAggregate>.Filter.Eq(p => p.OriginalId, notification.SourceId), Builders<CategoryAggregate>.Update.Inc(p => p.ProductCount, 1));
        }

        public async Task Handle(ProductRemoved notification)
        {
            if (notification == null)
            {
                throw new ArgumentNullException(nameof(notification));
            }

            var collection = _mongoClient.GetDatabase(_settings.Database).GetCollection<CategoryAggregate>(nameof(CategoryAggregate));
            await collection.UpdateOneAsync(Builders<CategoryAggregate>.Filter.Eq(p => p.OriginalId, notification.SourceId), Builders<CategoryAggregate>.Update.Inc(p => p.ProductCount, -1));
        }
    }
}
