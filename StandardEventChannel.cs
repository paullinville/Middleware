using System;
using System.Collections.Generic;

namespace MFB.API.Shared.Middleware.SMB
{
    public class StandardEventChannel
    {
        public string Name { get; }

        private StandardEventChannel()
        {
        }

        private StandardEventChannel(string name)
        {
            Name = name;
        }

        public static readonly StandardEventChannel Security = new StandardEventChannel("Security");
        public static readonly StandardEventChannel Validation = new StandardEventChannel("Validation");
        public static readonly StandardEventChannel Logging = new StandardEventChannel("Logging Event Channel");

        public static List<string> NameList()
        {
            List<string> lst = new List<string>();
            foreach (StandardEventChannel ITM in EventChannelList())
                lst.Add(ITM.Name);
            return lst;
        }

        public static List<StandardEventChannel> EventChannelList()
        {
            List<StandardEventChannel> lst = new List<StandardEventChannel>
            {
                Validation,
                Security,
                Logging
            };
            return lst;
        }

        public static StandardEventChannel EventChannelName(string name)
        {
            foreach (StandardEventChannel assfrm in EventChannelList())
            {
                if (assfrm.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    return assfrm;
            }
            return null;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}

