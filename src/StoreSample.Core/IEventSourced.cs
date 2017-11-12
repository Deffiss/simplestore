using System;
using System.Collections.Generic;

namespace StoreSample.Core
{
    public interface IEventSourced
    {
        Guid Id { get; }

        long Version { get; }

        IEnumerable<IVersionedEvent> Events { get; }
    }
}
