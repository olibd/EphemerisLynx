using System;
using PubnubApi;

namespace Lynx.Core.Services.Communications
{
    class PubNubCallback : SubscribeCallback
    {
        private event EventHandler<string> MessageReceived;

        public PubNubCallback(EventHandler<string> eventHandler)
        {
            MessageReceived += eventHandler;
        }

        public override void Status(Pubnub pubnub, PNStatus status)
        {
        }

        public override void Message<T>(Pubnub pubnub, PNMessageResult<T> message)
        {
            if (message.Message != null && MessageReceived.GetInvocationList().Length != 0)
            {
                //TODO: Check sender to ignore own messages
                MessageReceived(this, message.Message.ToString());
            }
        }

        public override void Presence(Pubnub pubnub, PNPresenceEventResult presence)
        {
        }
    }
}
