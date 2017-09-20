using System;
using Lynx.Core.Communications.Packets;
using Lynx.Core.Models.IDSubsystem;
using Newtonsoft.Json;
using NUnit.Framework;
using Attribute = Lynx.Core.Models.IDSubsystem.Attribute;

namespace CoreUnitTests.PCL
{
    [TestFixture()]
    public class AckTest : HandshakeTokenTest
    {
        [SetUp]
        public override void Setup()
        {
            base.Setup();

			//create some dummy attributes
			Attribute firstname = new Attribute()
			{
				Location = "1",
				Hash = "1",
                Description = "firstname",
				Content = new StringContent("Olivier")
			};

			Attribute lastname = new Attribute()
			{
				Location = "2",
				Hash = "2",
                Description = "lastname",
				Content = new StringContent("Brochu Dufour")
			};
            Attribute[] AccessibleAttr = new Attribute[]{firstname, lastname};

            IContent name = new StringContent("Olivier");
            _payload.Add("name", JsonConvert.SerializeObject(name));
            _token = new Ack(_header, _payload)
            {
                Id = _id,
                AccessibleAttributes = AccessibleAttr
            };
        }
    }
}
