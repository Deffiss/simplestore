using System;
using System.Collections.Generic;

namespace StoreSample.Core.Products
{
    public class CategoryChanged : VersionedEvent
    {
        public Guid CurrentCategoryId { get; set; }

        public Guid NewCategoryId { get; set; }

        public IDictionary<string, string> Properties { get; set; }
    }
}
