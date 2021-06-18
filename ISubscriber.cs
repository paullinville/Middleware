using System.Threading.Tasks;

namespace MFB.API.Shared.Middleware.SMB
{
    public interface ISubscriber
    {
        void ChannelNotification(ChannelEvent Evt);
        Task ChannelNotificationAsync(ChannelEvent Evt);
        int NotificationOrder();
    }
}

