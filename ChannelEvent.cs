using System;
using System.Threading.Tasks;

namespace MFB.API.Shared.Middleware.SMB
{
    [Serializable()]
    public class ChannelEvent : EventBase
    {
        public ChannelEvent(IPublisher Source, string Channel, string Name, object value) : base(Source)
        {
            base.Name = Name;
            base.Value = value;
            this.Channel = Channel;
        }

        public ChannelEvent(IPublisher Source, object value) : base(Source)
        {
            base.Name = Name;
            base.Value = value;
            this.Channel = this.GetType().FullName;
        }

        public ChannelEvent(IPublisher Source) : base(Source)
        {
            base.Name = Name;
            base.Value = null;
            this.Channel = this.GetType().FullName;
        }

        public ChannelEvent(IPublisher Source, StandardEventChannel Channel, string Name, object value) : base(Source)
        {
            base.Name = Name;
            base.Value = value;
            this.Channel = Channel.Name;
        }

        public ChannelEvent(IPublisher Source, StandardEventChannel Channel) : base(Source)
        {
            base.Name = "";
            base.Value = null;
            this.Channel = Channel.Name;
        }

        public ChannelEvent(IPublisher Source, StandardEventChannel Channel, string Name) : base(Source)
        {
            base.Name = Name;
            base.Value = null;
            this.Channel = Channel.Name;
        }

        public string Channel { get; } = "";

        public void SetSource(IPublisher srce)
        {
            if (Source == null)
            {
                Source = srce;
            }
        }

        public bool IsChannel(StandardEventChannel channel)
        {
            return channel.Name.Equals(this.Channel, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            return Channel + ":" + Name + Source.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj is ChannelEvent)
            {
                ChannelEvent other = (ChannelEvent)obj;
                if (other.Channel != Channel)
                    return false;
                else if (other.Name != Name)
                    return false;
                else if (!Source.Equals(other.Source))
                    return false;
                return true;
            }
            else
                return false;
        }

        public ChannelEvent Publish()
        {
            EventChannelBus.Publish(this);
            return this;
        }

        public async Task<ChannelEvent> PublishAsync()
        {
            await EventChannelBus.PublishAsync(this);
            return this;
        }

        public t Publish<t>() where t : ChannelEvent
        {
            EventChannelBus.Publish(this);
            return (t)this;
        }

        public async Task<t> PublishAsync<t>() where t : ChannelEvent
        {
            await EventChannelBus.PublishAsync(this);
            return (t)this;
        }

        public bool MustHandle { get; set; }

        public bool Handled { get; set; }
    }
}