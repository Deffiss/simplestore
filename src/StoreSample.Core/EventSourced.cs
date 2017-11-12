using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoreSample.Core
{
    public abstract class EventSourced : IEventSourced
    {
        private readonly Dictionary<Type, Action<IVersionedEvent>> _handlers = new Dictionary<Type, Action<IVersionedEvent>>();
        private readonly List<IVersionedEvent> _pendingEvents = new List<IVersionedEvent>();

        public Guid Id { get; }

        public long Version { get; private set; } = -1;

        public IEnumerable<IVersionedEvent> Events => _pendingEvents;

        protected EventSourced(Guid id)
        {
            Id = id;
        }

        protected void Handles<TEvent>(Action<TEvent> handler)
            where TEvent : IVersionedEvent
        {
            _handlers.Add(typeof(TEvent), @event => handler((TEvent)@event));
        }

        protected void LoadFrom(IEnumerable<IVersionedEvent> pastEvents)
        {
            foreach (var e in pastEvents)
            {
                _handlers[e.GetType()].Invoke(e);
                Version = e.Version;
            }
        }

        protected void Update(VersionedEvent e)
        {
            e.SourceId = Id;
            e.Version = Version + 1;

            _handlers[e.GetType()].Invoke(e);

            Version = e.Version;
            _pendingEvents.Add(e);
        }
    }
}
