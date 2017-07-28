using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PubnubApi;

namespace Lynx.Core.Services
{
    class PubnubCallback : SubscribeCallback
    {
        private event EventHandler<string> MessageReceived;

        public PubnubCallback(EventHandler<string> eventHandler)
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
