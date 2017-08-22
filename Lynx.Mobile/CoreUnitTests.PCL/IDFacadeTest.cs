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

			ID newId = await _idFacade.GetIDAsync(deployed.ControllerAddress);
			Assert.AreEqual(1, newId.Attributes.Count);
		}


		[Test]
		public async Task TestGetAccessibleAttributesAsync()
		{
			ID id = new ID();
			
			//ID deployed = await _idFacade.DeployAsync(new ID());
			Attribute attribute = new Attribute()
			{
				Location = "I am an atribute location",
				Hash = "I am an attribute hash"
			};
            id.AddAttribute("key1", attribute);
			//Bytes32TypeEncoder encoder = new Bytes32TypeEncoder();
			//byte[] keyBytes = encoder.Encode("key1");
			//Attribute addedAttrib = await _idFacade.AddAttributeAsync(deployed, keyBytes, attribute);
			Attribute attribute2 = new Attribute()
			{
				Location = "I am the second atribute location",
				Hash = "I am the second attribute hash"
			};
            id.AddAttribute("key2", attribute2);
            //byte[] keyBytes2 = encoder.Encode("key2");
            //Attribute addedAttrib2 = await _idFacade.AddAttributeAsync(deployed, keyBytes2, attribute2);
            id = await _idFacade.DeployAsync(id);
			ID newId = await _idFacade.GetIDAsync(id.ControllerAddress);
			Dictionary<string, Attribute> attributeDict = await _idFacade.GetAttributesAsync(newId);
			Assert.AreEqual(2, attributeDict.Count());
			//Assert.IsTrue(attributeDict.Keys.Contains(Encoding.UTF8.GetString(keyBytes, 0, keyBytes.Length)));

			/**Attribute attribute2 = new Attribute()
			{
				Location = "I am the second attribute location",
				Hash = "I am the second attribute hash"
			};
			byte[] keyBytes2 = encoder.Encode("test type2");
			Attribute addedAttrib2 = await _idFacade.AddAttributeAsync(deployed, keyBytes2, attribute2);
			Attribute attribute3 = new Attribute()
			{
				Location = "I am the third attribute location",
				Hash = "I am the third attribute hash"
			};
			byte[] keyBytes3 = encoder.Encode("test type3");
			Attribute addedAttrib3 = await _idFacade.AddAttributeAsync(deployed, keyBytes3, attribute3);

            ID newId = await _idFacade.GetIDAsync(deployed.ControllerAddress);
			string[] accessibleAttributes = new string[] { "test type1", "test type2" };
            Dictionary<string, Attribute> attributeDict = await _idFacade.GetAttributesAsync(newId);//, accessibleAttributes);
            Assert.AreEqual(1, attributeDict.Count());**/
			//Assert.AreEqual(accessibleAttributes, attributeDict.Keys.ToArray());
			//Assert.AreEqual(attribute1, attributeDict["test type1"]);
			//Assert.AreEqual(attribute2, attributeDict["test type2"]);
		}

		[Test]
		public async Task TestGetAttributesAsync()
		{
			ID deployed = await _idFacade.DeployAsync(new ID());
			Attribute attribute = new Attribute()
			{
				Location = "I am an atribute location",
				Hash = "I am an attribute hash"
			};

			Bytes32TypeEncoder encoder = new Bytes32TypeEncoder();
            byte[] keyBytes = encoder.Encode("key");
			Attribute addedAttrib = await _idFacade.AddAttributeAsync(deployed, keyBytes, attribute);

			ID newId = await _idFacade.GetIDAsync(deployed.ControllerAddress);
            Dictionary<string, Attribute> attributeDict = await _idFacade.GetAttributesAsync(newId);
            Assert.AreEqual(1, attributeDict.Count());
            Assert.IsTrue(attributeDict.Keys.Contains(Encoding.UTF8.GetString(keyBytes, 0, keyBytes.Length)));

            /**
			ID id = await _idFacade.DeployAsync(new ID());
            Bytes32TypeEncoder encoder = new Bytes32TypeEncoder();
			Attribute attribute1 = new Attribute()
			{
				Location = "I am the first attribute location",
				Hash = "I am the first attribute hash"
			};
            await _idFacade.AddAttributeAsync(id, encoder.Encode("test type1"), attribute1);
			//id.AddAttribute("test type1", attribute1);
			Attribute attribute2 = new Attribute()
			{
				Location = "I am the second attribute location",
				Hash = "I am the second attribute hash"
			};
			await _idFacade.AddAttributeAsync(id, encoder.Encode("test type2"), attribute2);
			Attribute attribute3 = new Attribute()
			{
				Location = "I am the third attribute location",
				Hash = "I am the third attribute hash"
			};
			await _idFacade.AddAttributeAsync(id, encoder.Encode("test type3"), attribute3);
            ID newId = await _idFacade.GetIDAsync(id.ControllerAddress);
			Dictionary<string, Attribute> attributeDict = await _idFacade.GetAttributesAsync(newId);
			Assert.AreEqual(3, newId.Attributes.Count);
			Assert.AreEqual(attribute1, attributeDict["test type1"]);
			Assert.AreEqual(attribute2, attributeDict["test type2"]);
            Assert.AreEqual(attribute3, attributeDict["test type3"]);**/
		}

		/**[Test]
        public async Task TestGetIDAsync()
        {
			ID id = await _idFacade.DeployAsync(new ID());
            Bytes32TypeEncoder encoder = new Bytes32TypeEncoder();
			Attribute attribute1 = new Attribute()
			{
				Location = "I am the first attribute location",
				Hash = "I am the first attribute hash"
			};
			id.AddAttribute("test type1", attribute1);
			Attribute attribute2 = new Attribute()
			{
				Location = "I am the second attribute location",
				Hash = "I am the second attribute hash"
			};
			id.AddAttribute("test type2", attribute2);
			Attribute attribute3 = new Attribute()
			{
				Location = "I am the third attribute location",
				Hash = "I am the third attribute hash"
			};
			id.AddAttribute("test type3", attribute3);
			id = await _idFacade.DeployAsync(id);
            ID id2 = await _idFacade.GetIDAsync(id.Address);
            Assert.AreEqual(id,id2);
            Assert.AreEqual(id.Attributes,id2.Attributes);
        }**/
	}
}
