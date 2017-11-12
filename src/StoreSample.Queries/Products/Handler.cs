using System.Threading.Tasks;
using MediatR;
using StoreSample.Core.Products;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using StoreSample.Queries.Settings;
using LinqKit;

namespace StoreSample.Queries.Products
{
    public class Handler :
        IAsyncRequestHandler<GetById, Product>,
        IAsyncRequestHandler<GetAll, (Product[] Products, long TotalCount)>,
        IAsyncNotificationHandler<Created>,
        IAsyncNotificationHandler<DescriptionUpdated>,
        IAsyncNotificationHandler<CategoryChanged>,
        IAsyncNotificationHandler<PropertiesChanged>,
        IAsyncNotificationHandler<Removed>,
        IAsyncNotificationHandler<Renamed>
    {
        private readonly MongoClient _mongoClient;
        private readonly MongoSettings _settings;

        public Handler(MongoClient mongoClient, IOptions<MongoSettings> settingsOptions)
        {
            _mongoClient = mongoClient;
            _settings = settingsOptions.Value;
        }

        public async Task<Product> Handle(GetById message)
        {
            if (message == null)
            {
                throw new System.ArgumentNullException(nameof(message));
            }

            var collection = _mongoClient.GetDatabase(_settings.Database).GetCollection<Product>(nameof(Product));
            return await collection.Find(p => p.OriginalId == message.Id).FirstOrDefaultAsync();
        }

        public async Task<(Product[] Products, long TotalCount)> Handle(GetAll message)
        {
            if (message == null)
            {
                throw new System.ArgumentNullException(nameof(message));
            }

            var filter = PredicateBuilder.New<Product>(true);

            if (message.CategoryId.HasValue)
            {
                filter.And(p => p.CategoryId == message.CategoryId);
            }

            var collection = _mongoClient.GetDatabase(_settings.Database).GetCollection<Product>(nameof(Product));
            return ((await collection.Find(filter).SortBy(p => p.Name).ToListAsync()).ToArray(), await collection.CountAsync(filter));
        }

        public async Task Handle(Created notification)
        {
            if (notification == null)
            {
                throw new System.ArgumentNullException(nameof(notification));
            }

            var collection = _mongoClient.GetDatabase(_settings.Database).GetCollection<Product>(nameof(Product));
            await collection.InsertOneAsync(new Product
            {
                OriginalId = notification.SourceId,
                CategoryId = notification.CategoryId,
                Name = notification.Name,
                Properties = notification.Properties
            });
        }

        public async Task Handle(DescriptionUpdated notification)
        {
            if (notification == null)
            {
                throw new System.ArgumentNullException(nameof(notification));
            }

            var collection = _mongoClient.GetDatabase(_settings.Database).GetCollection<Product>(nameof(Product));
            await collection.UpdateOneAsync(Builders<Product>.Filter.Eq(p => p.OriginalId, notification.SourceId), Builders<Product>.Update.Set(p => p.Description, notification.Description));
        }

        public async Task Handle(CategoryChanged notification)
        {
            if (notification == null)
            {
                throw new System.ArgumentNullException(nameof(notification));
            }

            var collection = _mongoClient.GetDatabase(_settings.Database).GetCollection<Product>(nameof(Product));
            await collection.UpdateOneAsync(Builders<Product>.Filter.Eq(p => p.OriginalId, notification.SourceId), 
                Builders<Product>.Update.Set(p => p.CategoryId, notification.NewCategoryId).Set(p => p.Properties, notification.Properties));
        }

        public async Task Handle(PropertiesChanged notification)
        {
            if (notification == null)
            {
                throw new System.ArgumentNullException(nameof(notification));
            }

            var collection = _mongoClient.GetDatabase(_settings.Database).GetCollection<Product>(nameof(Product));

            var updateBuilder = Builders<Product>.Update;
            UpdateDefinition<Product> updateDefinition = null;

            foreach (var prop in notification.Properties)
            {
                updateDefinition = updateDefinition == null ? updateBuilder.Set(p => p.Properties[prop.Key], prop.Value) : updateDefinition.Set(p => p.Properties[prop.Key], prop.Value);
            }

            await collection.UpdateOneAsync(Builders<Product>.Filter.Eq(p => p.OriginalId, notification.SourceId), updateDefinition);
        }

        public async Task Handle(Renamed notification)
        {
            if (notification == null)
            {
                throw new System.ArgumentNullException(nameof(notification));
            }

            var collection = _mongoClient.GetDatabase(_settings.Database).GetCollection<Product>(nameof(Product));
            await collection.UpdateOneAsync(Builders<Product>.Filter.Eq(p => p.OriginalId, notification.SourceId), Builders<Product>.Update.Set(p => p.Name, notification.Name));
        }

        public async Task Handle(Removed notification)
        {
            if (notification == null)
            {
                throw new System.ArgumentNullException(nameof(notification));
            }

            var collection = _mongoClient.GetDatabase(_settings.Database).GetCollection<Product>(nameof(Product));
            await collection.DeleteOneAsync(Builders<Product>.Filter.Eq(p => p.OriginalId, notification.SourceId));
        }
    }
}
