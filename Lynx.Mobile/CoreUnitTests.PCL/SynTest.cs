using System;
using Lynx.Core.Models.IDSubsystem;
using Lynx.Core.Services;
using Lynx.Core.Services.Communications;
using Lynx.Core.Services.Communications.Interfaces;
using Lynx.Core.Services.Communications.Packets;
using Lynx.Core.Services.Communications.Packets.Interfaces;
using Lynx.Core.Services.Interfaces;
using NUnit.Framework;
using Attribute = Lynx.Core.Models.IDSubsystem.Attribute;

namespace CoreUnitTests.PCL
{
    [TestFixture()]
    public class SynTest : HandshakeTokenTest
    {
        private ISession _session;

        [SetUp]
        public override void Setup()
        {
            base.Setup();
            _session = new PubNubSession(new EventHandler<string>((sender, e) => ProcessEncodedAck(e)));

            _payload.Add("netAddr", _session.Open());

            _token = new Syn()
            {
                Encrypted = Boolean.Parse(_header["encrypted"]),
                PublicKey = _header["pubkey"],
                NetworkAddress = _payload["netAddr"],
                Id = _id
            };
        }

        private IAck ProcessEncodedAck(string ack)
        {
            return null;
        }
    }
}
