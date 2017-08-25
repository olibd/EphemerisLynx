using System;
using PubnubApi;

namespace Lynx.Core.Communications
{
    class PubNubCallback : SubscribeCallback
    {
        PNConfiguration _config;

        private event EventHandler<string> MessageReceived;

        public PubNubCallback(EventHandler<string> eventHandler, PNConfiguration config)
        {
            MessageReceived += eventHandler;
            _config = config;
        }

        public override void Status(Pubnub pubnub, PNStatus status)
        {
        }

        public override void Message<T>(Pubnub pubnub, PNMessageResult<T> message)
        {
            if (message.Message != null && MessageReceived.GetInvocationList().Length != 0)
            {
                MessageReceived(this, message.Message.ToString());
            }
        }

        public override void Presence(Pubnub pubnub, PNPresenceEventResult presence)
        {
        }
    }
}
