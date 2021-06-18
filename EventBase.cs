using System.Collections.Generic;

namespace MFB.API.Shared.Middleware.SMB
{
    public abstract class EventBase
    {
        public EventBase(object Source)
        {
            this.Source = Source;
        }

        public object Source { get; protected set; }
        public string Name { get; protected set; }
        public object Value { get; protected set; }
        public Dictionary<string, object> AdditionalValues { get; set; }
    }
}

