using System;

namespace StoreSample.Core.Categories
{
    public class ProductAdded : VersionedEvent
    {
        public Guid ProductId { get; set; }
    }
}
