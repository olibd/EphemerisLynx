using System;
using Lynx.Core.Communications.Packets;
using Lynx.Core.Models.IDSubsystem;
using Newtonsoft.Json;
using NUnit.Framework;

namespace CoreUnitTests.PCL
{
    [TestFixture()]
    public class AckTest : HandshakeTokenTest
    {
        [SetUp]
        public override void Setup()
        {
            base.Setup();
            IContent name = new StringContent("Olivier");
            _payload.Add("name", JsonConvert.SerializeObject(name));
            _token = new Ack(_header, _payload)
            {
                //TODO: add accessible attributes property
                Id = _id
            };
        }
    }
}
