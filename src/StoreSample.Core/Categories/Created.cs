using System.Collections.Generic;

namespace StoreSample.Core.Categories
{
    public class Created : VersionedEvent
    {
        public string Name { get; set; }

        public IDictionary<string, string> Properties { get; set; }
    }
}
