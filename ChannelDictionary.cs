using System.Collections.Generic;

namespace MFB.API.Shared.Middleware.SMB
{
    public class ChannelDictionary : Dictionary<string, List<ISubscriber>>
    {
        public ChannelDictionary()
        {
            
        }
        public bool Enabled { get; set; } = true;
    }
}

