using System;
using System.Threading.Tasks;

namespace MFB.API.Shared.Middleware.SMB
{

    public class TypeSubscriber<t> : ISubscriber, IDisposable where t : ChannelEvent
    {
        private string ChannelName { get; }

        public TypeSubscriber(Action<t> handler)
        {
            ChannelName = typeof(t).FullName;
            EventChannelBus.Register(ChannelName, (ISubscriber)this);
            NotificationHandler = handler;
        }

        public TypeSubscriber(Func<t, Task> handler)
        {
            ChannelName = typeof(t).FullName;
            EventChannelBus.Register(ChannelName, (ISubscriber)this);
            AsyncHandler = handler;
        }

        public TypeSubscriber(Action<t> handler, Func<t, Task> ahandler)
        {
            ChannelName = typeof(t).FullName;
            EventChannelBus.Register(ChannelName, (ISubscriber)this);
            AsyncHandler = ahandler;
            NotificationHandler = handler;
        }

        protected virtual Action<t> NotificationHandler { get; set; }
        public virtual Func<t, Task> AsyncHandler { get; set; }

        public void ChannelNotification(ChannelEvent Evt)
        {
            if (Evt is t && NotificationHandler != null)
            {
                NotificationHandler.Invoke((t)Evt);
            }
            else
            {
                HandleOtherNotifications(Evt);
            }
        }

        protected virtual void HandleOtherNotifications(ChannelEvent evt)
        {
            //overide to handle other channelevents that are not type t.
        }

        public async Task ChannelNotificationAsync(ChannelEvent Evt)
        {
            if (Evt is t && AsyncHandler != null)
            {
                await AsyncHandler.Invoke((t)Evt);
            }
            else
            {
                await HandleOtherNotificationsAsync(Evt);
            }
        }

        protected async virtual Task HandleOtherNotificationsAsync(ChannelEvent evt)
        {
            await Task.FromResult(evt);
        }

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
                    EventChannelBus.Unregister(ChannelName, (ISubscriber)this);
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}