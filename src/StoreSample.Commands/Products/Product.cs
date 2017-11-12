using StoreSample.Commands.Categories;
using StoreSample.Commands.Services.Images;
using StoreSample.Commands.Services.Text;
using StoreSample.Core;
using StoreSample.Core.Products;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace StoreSample.Commands.Products
{
    public class Product : EventSourced
    {
        public string Name { get; private set; }

        public string Description { get; private set; }

        public string ImagePath { get; private set; }

        public bool IsDeleted { get; private set; }

        public Guid CategoryId { get; private set; }

        public IDictionary<string, string> Properties { get; private set; }

        protected Product(Guid id)
            : base(id)
        {
            Handles<Created>(OnCreated);
            Handles<DescriptionUpdated>(OnDescriptionUpdated);
            Handles<ImageUploaded>(OnImageUploaded);
            Handles<Removed>(OnRemoved);
            Handles<Renamed>(OnRenamed);
            Handles<CategoryChanged>(OnCategoryChanged);
            Handles<PropertiesChanged>(OnPropertiesChanged);
        }

        public Product(Guid id, string name, Category category)
            : this(id)
        {
            Update(new Created { CategoryId = category.Id, Name = name, Properties = category.Properties });
        }

        public Product(Guid id, IEnumerable<IVersionedEvent> history)
            : this(id)
        {
            LoadFrom(history);
        }

        public void Rename(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            Update(new Renamed { Name = name });
        }

        public void UpdateDescription(IHtmlSanitizer htmlSanitizer, string description)
        {
            if (htmlSanitizer == null)
            {
                throw new ArgumentNullException(nameof(htmlSanitizer));
            }

            if (string.IsNullOrEmpty(description))
            {
                throw new ArgumentNullException(nameof(description));
            }

            Update(new DescriptionUpdated { Description = description });
        }

        public void UpdateProperties(IDictionary<string, string> properties)
        {
            if (properties == null)
            {
                throw new ArgumentNullException(nameof(properties));
            }

            Update(new PropertiesChanged { Properties = properties });
        }

        public async Task UploadImage(IImageStoreService imageService, Stream fileStream, string extension)
        {
            if (imageService == null)
            {
                throw new ArgumentNullException(nameof(imageService));
            }

            if (fileStream == null)
            {
                throw new ArgumentNullException(nameof(fileStream));
            }

            if (string.IsNullOrEmpty(extension))
            {
                throw new ArgumentNullException(nameof(extension));
            }

            var path = await imageService.SaveImage(fileStream, CategoryId.ToString(), $"{Id.ToString()}.{extension}");
            Update(new ImageUploaded { ImagePath = path });
        }

        public void ChangeCategory(Category newCategory, IDictionary<string, string> properties = null)
        {
            if (newCategory == null)
            {
                throw new ArgumentNullException(nameof(newCategory));
            }

            // Merge properties with new category properties.
            var categoryProperties = newCategory.Properties.ToDictionary(p => p.Key, p => p.Value);
            foreach (var p in Properties)
            {
                if (categoryProperties.ContainsKey(p.Key))
                {
                    categoryProperties[p.Key] = p.Value;
                }
            }

            Update(new CategoryChanged { CurrentCategoryId = CategoryId, NewCategoryId = newCategory.Id, Properties = categoryProperties });

            if (properties != null)
            {
                UpdateProperties(properties);
            }
        }

        public void Remove()
        {
            Update(new Removed());
        }

        private void OnCreated(Created e)
        {
            Name = e.Name;
            Properties = e.Properties;
            CategoryId = e.CategoryId;
        }

        private void OnRenamed(Renamed e) => Name = e.Name;

        private void OnPropertiesChanged(PropertiesChanged e)
        {
            foreach (var p in e.Properties)
            {
                Properties[p.Key] = p.Value;
            }
        }

        private void OnCategoryChanged(CategoryChanged e)
        {
            CategoryId = e.NewCategoryId;
            Properties = e.Properties;
        }

        private void OnDescriptionUpdated(DescriptionUpdated e) => Description = e.Description;

        private void OnRemoved(Removed e) => IsDeleted = true;

        private void OnImageUploaded(ImageUploaded e) => ImagePath = e.ImagePath;
    }
}
