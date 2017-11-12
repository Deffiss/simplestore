using StoreSample.Commands.Products;
using StoreSample.Core;
using StoreSample.Core.Categories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StoreSample.Commands.Categories
{
    public class Category : EventSourced
    {
        private HashSet<Guid> _productIds = new HashSet<Guid>();

        public string Name { get; private set; }

        public IDictionary<string, string> Properties { get; private set; } = new Dictionary<string, string>();

        public IEnumerable<Guid> ProductIds => _productIds;

        protected Category(Guid id)
            : base(id)
        {
            Handles<Created>(OnCreated);
            Handles<Renamed>(OnRenamed);
            Handles<PropertiesChanged>(OnPropertiesChanged);
            Handles<ProductAdded>(OnProductAdded);
            Handles<ProductRemoved>(OnProductRemoved);
        }

        public Category(Guid id, IEnumerable<IVersionedEvent> history)
            : this(id)
        {
            LoadFrom(history);
        }

        public Category(Guid id, string name, IDictionary<string, string> properties)
            : this(id)
        {
            Update(new Created { Name = name, Properties = properties });
        }

        public void Rename(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            Update(new Renamed { Name = name });
        }

        public void ChangeProperties(IDictionary<string, string> properties)
        {
            if (properties == null)
            {
                throw new ArgumentNullException(nameof(properties));
            }

            Update(new PropertiesChanged { Properties = properties });
        }

        public void AddProduct(Product product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            Update(new ProductAdded { ProductId = product.Id });
        }

        public void RemoveProduct(Product product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            Update(new ProductRemoved { ProductId = product.Id });
        }

        private void OnCreated(Created e)
        {
            Name = e.Name;
            Properties = e.Properties;
        }

        private void OnRenamed(Renamed e)
        {
            Name = e.Name;
        }

        private void OnPropertiesChanged(PropertiesChanged e)
        {
            Properties = e.Properties;
        }

        private void OnProductRemoved(ProductRemoved e)
        {
            _productIds.Remove(e.ProductId);
        }

        private void OnProductAdded(ProductAdded e)
        {
            _productIds.Add(e.ProductId);
        }
    }
}
