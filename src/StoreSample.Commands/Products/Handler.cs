using System.Threading.Tasks;
using MediatR;
using StoreSample.Commands.Repositories;
using StoreSample.Commands.Services.Images;
using StoreSample.Commands.Services.Text;
using System;
using System.IO;
using System.Linq;

namespace StoreSample.Commands.Products
{
    public class Handler :
        IAsyncRequestHandler<Create>,
        IAsyncRequestHandler<Update>,
        IAsyncRequestHandler<UploadImage>,
        IAsyncRequestHandler<Remove>,
        IAsyncNotificationHandler<Core.Categories.PropertiesChanged>
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Categories.Category> _categoryRepository;
        private readonly IImageStoreService _imageStoreService;
        private readonly IHtmlSanitizer _htmlSanitizer;

        public Handler(
            IRepository<Product> productRepository,
            IRepository<Categories.Category> categoryRepository,
            IImageStoreService imageStoreService,
            IHtmlSanitizer htmlSanitizer)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _imageStoreService = imageStoreService;
            _htmlSanitizer = htmlSanitizer;
        }

        public async Task Handle(Create message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            // Create a new product for the particular category.
            var category = await _categoryRepository.Get(message.ProductData.CategoryId);
            var product = new Product(message.Id, message.ProductData.Name, category);

            if (!string.IsNullOrEmpty(message.ProductData.Description))
            {
                product.UpdateDescription(_htmlSanitizer, message.ProductData.Description);
            }

            if (message.ProductData.Properties != null)
            {
                product.UpdateProperties(message.ProductData.Properties);
            }

            // Store the product.
            await _productRepository.Save(product);
        }

        public async Task Handle(Update message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var product = await _productRepository.Get(message.Id);

            // Check if renamed.
            if (!string.IsNullOrEmpty(message.ProductData.Name))
            {
                product.Rename(message.ProductData.Name);
            }

            if (!string.IsNullOrEmpty(message.ProductData.Description))
            {
                product.UpdateDescription(_htmlSanitizer, message.ProductData.Description);
            }

            if (message.ProductData.CategoryId is Guid catId)
            {
                var newCategory = await _categoryRepository.Get(catId);
                product.ChangeCategory(newCategory, message.ProductData.Properties);
            }
            else if (message.ProductData.Properties != null)
            {
                product.UpdateProperties(message.ProductData.Properties);
            }

            await _productRepository.Save(product);
        }


        public async Task Handle(UploadImage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var product = await _productRepository.Get(message.Id);
            await product.UploadImage(_imageStoreService, message.File.OpenReadStream(), Path.GetExtension(message.File.FileName));

            await _productRepository.Save(product);
        }

        public async Task Handle(Remove message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var product = await _productRepository.Get(message.Id);
            product.Remove();

            await _productRepository.Save(product);
        }

        public async Task Handle(Core.Categories.PropertiesChanged notification)
        {
            if (notification == null)
            {
                throw new ArgumentNullException(nameof(notification));
            }

            var category = await _categoryRepository.Get(notification.SourceId);
            var products = await Task.WhenAll(category.ProductIds.Select(pid => _productRepository.Get(pid)));

            foreach (var product in products)
            {
                product.ChangeCategory(category);
            }

            await Task.WhenAll(products.Select(p => _productRepository.Save(p)));
        }
    }
}
