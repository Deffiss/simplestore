using System.Collections.Generic;

namespace StoreSample.Core.Categories
{
    public class PropertiesChanged : VersionedEvent
    {
        public IDictionary<string, string> Properties { get; set; }
    }
}
