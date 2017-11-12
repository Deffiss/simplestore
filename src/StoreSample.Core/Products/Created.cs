using System;
using System.Collections.Generic;

namespace StoreSample.Core.Products
{
    public class Created : VersionedEvent
    {
        public string Name { get; set; }

        public Guid CategoryId { get; set; }

        public IDictionary<string, string> Properties { get; set; }
    }
}
