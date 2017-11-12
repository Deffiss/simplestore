using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoreSample.Core
{
    public interface IVersionedEvent : INotification
    {
        Guid SourceId { get; }

        long Version { get; }
    }
}
