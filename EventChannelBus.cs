using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MFB.API.Shared.Middleware.SMB
{
    public class EventChannelBus
    {
        private static Func<string> GetContextFunc;

        private static IHttpContextAccessor HttpContext;

        public static void SetContextFunc(Func<string> cntxt)
        {
            GetContextFunc = cntxt;
            RequestChannels.TryAdd(GetContextFunc.Invoke(), new ChannelDictionary());
        }

        public static void SetContext(IHttpContextAccessor context)
        {
            HttpContext = context;
        }

        public static string SessionID()
        {
            if (GetContextFunc != null)
                return GetContextFunc();
            else if (HttpContext != null)
                return HttpContext.HttpContext.TraceIdentifier;
            else
                return "DefaultContext-a0etgr9";
        }

        public static bool Enabled
        {
            get
            {
                return Channels.Enabled;
            }
        }

        public static void Enable()
        {
            Channels.Enabled = true;
        }

        public static void Disable()
        {
            Channels.Enabled = false;
        }

        public static void DeleteChannels(string sessionId)
        {
            ChannelDictionary defaultDic = new ChannelDictionary();
            if (RequestChannels.TryGetValue(sessionId, out defaultDic))
                defaultDic.Clear();
            ChannelDictionary removed;
            RequestChannels.TryRemove(sessionId, out removed);
        }

        private static ConcurrentDictionary<string, ChannelDictionary> RequestChannels { get; set; } = new ConcurrentDictionary<string, ChannelDictionary>(3, 10);

        public static void Register(string Channel, ISubscriber Subscriber)
        {
            if (!Enabled)
                return;
            if (!Channels.ContainsKey(Channel))
                Channels.Add(Channel, new List<ISubscriber>());

            if (!Channels[Channel].Contains(Subscriber))
                Channels[Channel].Add(Subscriber);
        }

        public static void Register(Type Channel, ISubscriber Subscriber)
        {
            Register(Channel.FullName, Subscriber);
        }

        public static void Register(object Channel, ISubscriber Subscriber)
        {
            Register(Channel.GetType(), Subscriber);
        }

        public static void Unregister(string Channel, ISubscriber Subscriber)
        {
            if (!Enabled)
                return;
            if (Channels.ContainsKey(Channel))
            {
                if (Channels[Channel].Contains(Subscriber))
                    Channels[Channel].Remove(Subscriber);
                // delete the list if no subscribers are left
                if (Channels[Channel].Count == 0)
                    Channels.Remove(Channel);
            }
        }

        public static void Unregister(StandardEventChannel Channel, ISubscriber Subscriber)
        {
            if (!Enabled)
                return;
            Unregister(Channel.Name, Subscriber);
        }

        public static void UnregisterAll(ISubscriber Subscriber)
        {
            if (!Enabled)
                return;
            List<string> emptyChannelsKeys = new List<string>();
            foreach (string channel in Channels.Keys)
            {
                if (Channels[channel].Contains(Subscriber))
                    Channels[channel].Remove(Subscriber);
                if (Channels[channel].Count == 0)
                    emptyChannelsKeys.Add(channel);
            }

            foreach (string channel in emptyChannelsKeys)
                Channels.Remove(channel);
        }

        private static void NotifySubscribers(string Channel, ChannelEvent Evt)
        {
            if (!Enabled)
                return;
            if (Channels.ContainsKey(Channel))
            {
                foreach (ISubscriber evt in Channels[Channel].OrderBy(x => x.NotificationOrder()))
                {
                    evt.ChannelNotification(Evt);
                }
            }
        }

        public static void Publish(IPublisher Publisher, string Channel, string Name, object value)
        {
            if (!Enabled)
                return;
            ChannelEvent evt = new ChannelEvent(Publisher, Channel, Name, value);
            NotifySubscribers(Channel, evt);
        }

        public static void Publish(IPublisher Publisher, StandardEventChannel Channel, string Name, object value)
        {
            if (!Enabled)
                return;
            Publish(Publisher, Channel.Name, Name, value);
        }

        public static void Publish(ChannelEvent evt)
        {
            if (!Enabled)
                return;
            NotifySubscribers(evt.Channel, evt);
            if (evt.MustHandle && !evt.MustHandle)
            {
                throw new ApplicationException("Channel required channel event was not handled.");
            }
        }

        public static t Publish<t>(t evt) where t : ChannelEvent
        {
            if (Enabled)
                Publish(evt);
            return evt;
        }

        private async static Task NotifySubscribersAsync(string Channel, ChannelEvent Evt)
        {
            if (!Enabled)
                return;
            string id = SessionID();
            if (Channels.ContainsKey(Channel))
            {
                foreach (ISubscriber evt in Channels[Channel].OrderBy(x => x.NotificationOrder()))
                {
                    await evt.ChannelNotificationAsync(Evt);
                }
            }
            else
            {
                Console.WriteLine($"{Channel} not subscribed for id {id}");
            }
        }

        private static ChannelDictionary Channels
        {
            get
            {
                ChannelDictionary dic = new ChannelDictionary();
                return RequestChannels.GetOrAdd(SessionID(), dic);
            }
        }

        public async static Task PublishAsync(IPublisher Publisher, string Channel, string Name, object value)
        {
            if (!Enabled)
                return;
            ChannelEvent evt = new ChannelEvent(Publisher, Channel, Name, value);
            await NotifySubscribersAsync(Channel, evt);
        }

        public async static Task PublishAsync(IPublisher Publisher, StandardEventChannel Channel, string Name, object value)
        {
            if (!Enabled)
                return;
            await PublishAsync(Publisher, Channel.Name, Name, value);
        }

        public async static Task PublishAsync(ChannelEvent evt)
        {
            if (!Enabled)
                return;
            await NotifySubscribersAsync(evt.Channel, evt);
            if (evt.MustHandle && !evt.MustHandle)
            {
                throw new ApplicationException("Channel required channel event was not handled.");
            }
        }

        public async static Task<t> PublishAsync<t>(t evt) where t : ChannelEvent
        {
            if (Enabled)
                await PublishAsync(evt);
            return evt;
        }
    }
}