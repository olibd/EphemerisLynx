using System;
using Lynx.Core.Communications;
using Lynx.Core.Communications.Interfaces;
using Lynx.Core.Communications.Packets;
using Lynx.Core.Communications.Packets.Interfaces;
using Lynx.Core.Models.IDSubsystem;
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
            _session = new AblySession(new EventHandler<string>((sender, e) => ProcessEncodedAck(e)), "123");

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
