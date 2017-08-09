using System;
using System.Text;
using Lynx.Core.Communications.Interfaces;
using Nethereum.ABI.Util;
using Nethereum.KeyStore.Crypto;
using PubnubApi;

namespace Lynx.Core.Communications
{
    public class PubNubSession : ISession
    {
        private readonly Pubnub _pubNub;

        private string _channel;

        public PubNubSession(EventHandler<string> messageEventHandler)
        {
            PNConfiguration pubNubConfig = new PNConfiguration()
            {
                PublishKey = "pub-c-361ee194-a500-4a92-bc1f-54b0ece5ee3d",
                SubscribeKey = "sub-c-6d071204-6b3a-11e7-b6db-02ee2ddab7fe"
            };

            _pubNub = new Pubnub(pubNubConfig);
            AddMessageReceptionHandler(messageEventHandler);
        }

        public void AddMessageReceptionHandler(EventHandler<string> handler)
        {
            _pubNub.AddListener(new PubNubCallback(handler));
        }

        public void Close()
        {
            if (String.IsNullOrEmpty(_channel))
                throw new ConnectOperationImpossibleException("Trying to close a connection while no connection exists");

            _pubNub.Unsubscribe<string>()
                .Channels(new[] { _channel })
                .Execute();

            _channel = "";
        }

        public void Open(string channelName)
        {
            SubscribeChannel(channelName);
        }

        private void SubscribeChannel(string channelName)
        {
            if (!String.IsNullOrEmpty(_channel))
                throw new ConnectOperationImpossibleException("Trying to open a connection while one already exists");

            _channel = channelName;

            _pubNub.Subscribe<string>()
                .Channels(new[] { _channel })
                .Execute();
        }

        public string Open()
        {
            SubscribeChannel(GenerateChannelName());
            return _channel;
        }

        public void Send(string message)
        {
            _pubNub.Publish()
                .Channel(_channel)
                .Message(message)
                .ShouldStore(false)
                .UsePOST(true)
                .Async(new PNPublishResultExt(
                    (result, status) =>
                    {
                        if (status.Error)
                        {
                            throw status.ErrorData.Throwable;
                        }
                    }
                    ));
        }


        private string GenerateChannelName()
        {
            RandomBytesGenerator rng = new RandomBytesGenerator();
            Sha3Keccack hasher = new Sha3Keccack();
            string key = Encoding.UTF8.GetString(rng.GenerateRandomBytes(32), 0, 32);
            return hasher.CalculateHash(key);

        }
    }
}
