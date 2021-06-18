using System;
using System.Threading.Tasks;

namespace MFB.API.Shared.Middleware.SMB
{
    public abstract class SubscriberBase : ISubscriber, IDisposable
    {

        private string ChannelName { get; }
        public SubscriberBase(string channelName)
        {
            ChannelName = channelName;
            EventChannelBus.Register(ChannelName, this);
        }

        public SubscriberBase(StandardEventChannel channel) : this(channel.Name)
        {
        }

        public abstract void ChannelNotification(ChannelEvent Evt);


        public virtual int NotificationOrder()
        {
            return 1;
        }

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    EventChannelBus.Unregister(ChannelName, this);
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {

            Dispose(true);
        }

        public abstract Task ChannelNotificationAsync(ChannelEvent Evt);




    }
}

