using System.Collections.Generic;

namespace StoreSample.Core.Products
{
    public class PropertiesChanged : VersionedEvent
    {
        public IDictionary<string, string> Properties { get; set; }
    }
}
