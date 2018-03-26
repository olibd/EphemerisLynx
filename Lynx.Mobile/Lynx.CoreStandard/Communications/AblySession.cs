using System;
using System.Collections.Generic;
using System.Text;
using IO.Ably;
using IO.Ably.Realtime;
using Lynx.Core.Communications.Interfaces;
using Nethereum.ABI.Util;
using Nethereum.KeyStore.Crypto;

namespace Lynx.Core.Communications
{
    public class AblySession : ISession
    {
        private AblyRealtime _ably;
        private IRealtimeChannel _channel;
        private List<EventHandler<string>> _handlers;
        private string _publicAddress;

        public AblySession(EventHandler<string> handler, string publicAddress)
        {
            _ably = new AblyRealtime("NYAqNA.AD_snA:CgTo0xPWXieVEH-h");
            _handlers = new List<EventHandler<string>>();
            _handlers.Add(handler);
            _publicAddress = publicAddress;
        }

        public string SessionID { get { return _channel.Name; } }

        public void AddMessageReceptionHandler(EventHandler<string> handler)
        {
            _handlers.Add(handler);
        }

        public void ClearMessageReceptionHandlers()
        {
            _handlers.Clear();
        }

        public void Close()
        {
            _channel.Unsubscribe();
            _ably.Close();
        }

        public void Open(string networkAddress)
        {
            _channel = _ably.Channels.Get(networkAddress);
            _channel.Subscribe((Message msg) =>
            {
                if (_publicAddress != msg.Name)
                {
                    foreach (EventHandler<string> handler in _handlers)
                    {
                        handler.Invoke(this, msg.Data.ToString());
                    }
                }
            });
        }

        public string Open()
        {
            Open(GenerateChannelName());
            return _channel.Name;
        }

        public void Send(string message)
        {
            _channel.Publish(_publicAddress, message);
        }

        private string GenerateChannelName()
        {
            byte[] randomBytes = new byte[32];
            Random rng = new Random();
            rng.NextBytes(randomBytes);
            Sha3Keccack hasher = new Sha3Keccack();
            string key = Encoding.UTF8.GetString(randomBytes, 0, 32);
            string hash = hasher.CalculateHash(key);
            return hash.Substring(0, 7);
        }
    }
}
