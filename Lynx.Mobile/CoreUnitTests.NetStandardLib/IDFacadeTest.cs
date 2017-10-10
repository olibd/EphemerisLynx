using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eVi.abi.lib.pcl;
using Lynx.Core.Facade;
using Lynx.Core.Facade.Interfaces;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.ABI.Encoders;
using NUnit.Framework;
using Lynx.Core.Models.IDSubsystem;
using System.Text;


namespace CoreUnitTests.PCL
{
    [TestFixture]
    class IDFacadeTest : FacadeTest
    {
        private ICertificateFacade _certFacade;
        private IAttributeFacade _attributeFacade;
        private IIDFacade _idFacade;

        [SetUp]
        public void Setup()
        {
            SetupAsync().Wait();

            _certFacade = new CertificateFacade(_web3, new DummyContentService(), _accountService);
            _attributeFacade = new AttributeFacade(_web3, _certFacade, new DummyContentService(), _accountService);
            _idFacade = new IDFacade(_factoryService.GetAddress(), _web3, _attributeFacade, _accountService);
        }

        [Test]
        public async Task TestDeploy()
        {
            Attribute attribute = new Attribute()
            {
                Location = "I am an atribute location",
                Hash = "I am an attribute hash",
                Content = new StringContent("Ephemeris"),
                Description = "test type"
            };
            ID id = new ID();
            id.AddAttribute(attribute);
            id = await _idFacade.DeployAsync(id);
            Assert.NotNull(id.Address);
            Assert.NotNull(attribute.Address);
            Assert.NotNull(attribute.Owner);
        }

        [Test]
        public async Task TestAddAndGetAttributes()
        {
            ID deployed = await _idFacade.DeployAsync(new ID());
            Attribute attribute = new Attribute()
            {
                Location = "I am an atribute location",
                Hash = "I am an attribute hash",
                Content = new StringContent("Ephemeris"),
                Description = "key"
            };

            Attribute addedAttrib = await _idFacade.AddAttributeAsync(deployed, attribute);

            Assert.NotNull(addedAttrib.Address);

            ID newId = await _idFacade.GetIDAsync(deployed.Address);
            Assert.AreEqual(1, newId.Attributes.Count);
        }


        [Test]
        public async Task TestGetAccessibleAttributesAsync()
        {
            ID id = new ID();

            Attribute attribute = new Attribute()
            {
                Location = "I am an atribute location",
                Hash = "I am an attribute hash",
                Description = "key1"
            };
            id.AddAttribute(attribute);

            Attribute attribute2 = new Attribute()
            {
                Location = "I am the second atribute location",
                Hash = "I am the second attribute hash",
                Description = "key2"
            };
            id.AddAttribute(attribute2);

            Attribute attribute3 = new Attribute()
            {
                Location = "I am the third atribute location",
                Hash = "I am the third attribute hash",
                Description = "key3"
            };
            id.AddAttribute(attribute3);

            id = await _idFacade.DeployAsync(id);
            ID newId = await _idFacade.GetIDAsync(id.Address);
            string[] accessibleAttributes = new string[] { "key1", "key3" };
            Dictionary<string, Attribute> attributeDict = await _idFacade.GetAttributesAsync(newId, accessibleAttributes);
            Assert.AreEqual(2, attributeDict.Count());
            Assert.AreEqual(accessibleAttributes, attributeDict.Keys.ToArray());
        }

        [Test]
        public async Task TestGetAttributesAsync()
        {
            ID id = new ID();

            Attribute attribute = new Attribute()
            {
                Location = "I am an atribute location",
                Hash = "I am an attribute hash",
                Description = "key1"
            };
            id.AddAttribute(attribute);

            Attribute attribute2 = new Attribute()
            {
                Location = "I am the second atribute location",
                Hash = "I am the second attribute hash",
                Description = "key2"
            };
            id.AddAttribute(attribute2);

            id = await _idFacade.DeployAsync(id);
            ID newId = await _idFacade.GetIDAsync(id.Address);
            Dictionary<string, Attribute> attributeDict = await _idFacade.GetAttributesAsync(newId);
            Assert.AreEqual(2, attributeDict.Count());
        }

        [Test]
        public async Task TestGetIDAsync()
        {
            ID id = new ID();

            Attribute attribute = new Attribute()
            {
                Location = "I am an atribute location",
                Hash = "I am an attribute hash",
                Description = "key1"
            };
            id.AddAttribute(attribute);

            Attribute attribute2 = new Attribute()
            {
                Location = "I am the second atribute location",
                Hash = "I am the second attribute hash",
                Description = "key2"
            };
            id.AddAttribute(attribute2);

            Attribute attribute3 = new Attribute()
            {
                Location = "I am the third atribute location",
                Hash = "I am the third attribute hash",
                Description = "key3"
            };
            id.AddAttribute(attribute3);

            id = await _idFacade.DeployAsync(id);
            string[] accessibleAttributes = new string[] { "key1", "key3" };
            ID newId = await _idFacade.GetIDAsync(id.Address, accessibleAttributes);
            Assert.AreEqual(2, newId.Attributes.Count());
            Assert.AreEqual(accessibleAttributes, newId.Attributes.Keys.ToArray());
        }

    }
}
