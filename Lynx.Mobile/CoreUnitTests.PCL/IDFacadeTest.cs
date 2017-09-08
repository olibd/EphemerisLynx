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

            Task<string> deployFactory = DeployFactory();
            deployFactory.Wait();

            _certFacade = new CertificateFacade(_addressFrom, "", _web3, new DummyContentService());
            _attributeFacade = new AttributeFacade(_addressFrom, "", _web3, _certFacade, new DummyContentService());
            _idFacade = new IDFacade(_addressFrom, "", deployFactory.Result, _web3, _attributeFacade);
        }

        private async Task<string> DeployFactory()
        {
            string transactionHash = await FactoryService.DeployContractAsync(_web3, _addressFrom, new HexBigInteger(3905820));
            TransactionReceipt receipt = await _web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);
            return receipt.ContractAddress;
        }

        [Test]
        public async Task TestDeploy()
        {
            Attribute attribute = new Attribute()
            {
                Location = "I am an atribute location",
                Hash = "I am an attribute hash"
            };
            ID id = new ID();
            id.AddAttribute("test type", attribute);
            id = await _idFacade.DeployAsync(id);
            Assert.NotNull(id.Address);
            Assert.NotNull(attribute.Address);
        }

        [Test]
        public async Task TestAddAndGetAttributes()
        {
            ID deployed = await _idFacade.DeployAsync(new ID());
            Attribute attribute = new Attribute()
            {
                Location = "I am an atribute location",
                Hash = "I am an attribute hash"
            };

            Bytes32TypeEncoder encoder = new Bytes32TypeEncoder();
            Attribute addedAttrib = await _idFacade.AddAttributeAsync(deployed, encoder.Encode("key"), attribute);

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
                Hash = "I am an attribute hash"
            };
            id.AddAttribute("key1", attribute);

            Attribute attribute2 = new Attribute()
            {
                Location = "I am the second atribute location",
                Hash = "I am the second attribute hash"
            };
            id.AddAttribute("key2", attribute2);

            Attribute attribute3 = new Attribute()
            {
                Location = "I am the third atribute location",
                Hash = "I am the third attribute hash"
            };
            id.AddAttribute("key3", attribute3);

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
                Hash = "I am an attribute hash"
            };
            id.AddAttribute("key1", attribute);

            Attribute attribute2 = new Attribute()
            {
                Location = "I am the second atribute location",
                Hash = "I am the second attribute hash"
            };
            id.AddAttribute("key2", attribute2);

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
                Hash = "I am an attribute hash"
            };
            id.AddAttribute("key1", attribute);

            Attribute attribute2 = new Attribute()
            {
                Location = "I am the second atribute location",
                Hash = "I am the second attribute hash"
            };
            id.AddAttribute("key2", attribute2);

            Attribute attribute3 = new Attribute()
            {
                Location = "I am the third atribute location",
                Hash = "I am the third attribute hash"
            };
            id.AddAttribute("key3", attribute3);

            id = await _idFacade.DeployAsync(id);
            string[] accessibleAttributes = new string[] { "key1", "key3" };
            ID newId = await _idFacade.GetIDAsync(id.Address, accessibleAttributes);
            Assert.AreEqual(2, newId.Attributes.Count());
            Assert.AreEqual(accessibleAttributes, newId.Attributes.Keys.ToArray());
        }

    }
}
