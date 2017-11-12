using System;

namespace StoreSample.Core
{
    public abstract class VersionedEvent : IVersionedEvent
    {
        public Guid SourceId { get; set; }

        public long Version { get; set; }
    }
}
