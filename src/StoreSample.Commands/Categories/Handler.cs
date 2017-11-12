using System.Threading.Tasks;
using MediatR;
using System;
using StoreSample.Commands.Repositories;

namespace StoreSample.Commands.Categories
{
    public class Handler :
        IAsyncRequestHandler<Create>,
        IAsyncRequestHandler<Update>,
        IAsyncNotificationHandler<Core.Products.Created>,
        IAsyncNotificationHandler<Core.Products.Removed>,
        IAsyncNotificationHandler<Core.Products.CategoryChanged>
    {
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Products.Product> _productRepository;

        public Handler(IRepository<Category> categoryRepository, IRepository<Products.Product> productRepository)
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
        }

        public async Task Handle(Create message)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            // Create the category and store it.
            // Event source repository will take care about the rest.
            var category = new Category(message.Id, message.CategoryData.Name, message.CategoryData.Properties);
            await _categoryRepository.Save(category);
        }
        public async Task Handle(Update message)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var category = await _categoryRepository.Get(message.Id);

            // If the name was provided then rename the product.
            if (!string.IsNullOrEmpty(message.CategoryData.Name))
            {
                category.Rename(message.CategoryData.Name);
            }

            // If new properties was configured then update category properties.
            if (message.CategoryData.Properties != null)
            {
                category.ChangeProperties(message.CategoryData.Properties);
            }

            await _categoryRepository.Save(category);
        }

        public async Task Handle(Core.Products.Created notification)
        {
            if (notification == null)
            {
                throw new ArgumentNullException(nameof(notification));
            }

            var categoryTask =  _categoryRepository.Get(notification.CategoryId);
            var productTask = _productRepository.Get(notification.SourceId);

            await Task.WhenAll(categoryTask, productTask);

            var category = categoryTask.Result;
            var product = productTask.Result;

            category.AddProduct(product);

            await _categoryRepository.Save(category);
        }

        public async Task Handle(Core.Products.Removed notification)
        {
            if (notification == null)
            {
                throw new ArgumentNullException(nameof(notification));
            }

            var product = await _productRepository.Get(notification.SourceId);
            var category = await _categoryRepository.Get(product.CategoryId);

            category.RemoveProduct(product);

            await _categoryRepository.Save(category);
        }

        public async Task Handle(Core.Products.CategoryChanged notification)
        {
            if (notification == null)
            {
                throw new ArgumentNullException(nameof(notification));
            }

            var oldCategoryTask = _categoryRepository.Get(notification.CurrentCategoryId);
            var newCategoryTask = _categoryRepository.Get(notification.NewCategoryId);
            var productTask = _productRepository.Get(notification.SourceId);

            await Task.WhenAll(oldCategoryTask, newCategoryTask, productTask);

            var oldCategory = oldCategoryTask.Result;
            var newCategory = newCategoryTask.Result;
            var product = productTask.Result;

            oldCategory.RemoveProduct(product);
            newCategory.AddProduct(product);

            await Task.WhenAll(_categoryRepository.Save(oldCategory), _categoryRepository.Save(newCategory));
        }
    }
}
