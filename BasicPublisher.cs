using MFB.API.Shared.Middleware.SMB;
namespace MFB.API.Shared.Middleware.SMB
{
    public class BasicPublisher : IPublisher
    {
        public BasicPublisher(object source, ChannelEvent evt)
        {
            evt.SetSource(this);
            ChannelEvent = evt;
            ChannelEvent.Publish();
        }

        public ChannelEvent ChannelEvent { get; }
    }
}