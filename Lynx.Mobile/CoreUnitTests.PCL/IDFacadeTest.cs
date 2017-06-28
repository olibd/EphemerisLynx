using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eVi.abi.lib.pcl;
using Lynx.Core.Facade;
using Lynx.Core.Facade.Interfaces;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.ABI.Encoders;
using NUnit.Framework;
using Lynx.Core.Models.IDSubsystem;

namespace CoreUnitTests.PCL
{
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

            _certFacade = new CertificateFacade(_addressFrom, "", _web3);
            _attributeFacade = new AttributeFacade(_addressFrom, "", _web3, _certFacade);
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

            Bytes32TypeEncoder Encoder = new Bytes32TypeEncoder();
            Attribute addedAttrib = await _idFacade.AddAttributeAsync(deployed, Encoder.Encode("key"), attribute);

            ID newId = await _idFacade.GetIDAsync(deployed.Address);
            Assert.AreEqual(1, newId.Attributes.Count);
        }

    }
}
