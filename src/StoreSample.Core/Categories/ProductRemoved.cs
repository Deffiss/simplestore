using System;

namespace StoreSample.Core.Categories
{
    public class ProductRemoved : VersionedEvent
    {
        public Guid ProductId { get; set; }
    }
}
