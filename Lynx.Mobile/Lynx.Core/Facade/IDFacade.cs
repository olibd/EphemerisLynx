using Lynx.Core.Facade.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nethereum.Web3;
using Lynx.Core.Models.IDSubsystem;
using eVi.abi.lib.pcl;
using Nethereum.Hex.HexTypes;
using System.Numerics;

namespace Lynx.Core.Facade
{
    public class IDFacade : Facade, IIDFacade
    {
        private string _factoryAddress;
        private IAttributeFacade _attributeFacade;

        public IDFacade(string address, string password, string factoryAddress, Web3 web3, IAttributeFacade attributeFacade) : base(address, password, web3)
        {
            _factoryAddress = factoryAddress;
            _attributeFacade = attributeFacade;
        }

        public IDFacade(string address, string password, string factoryAddress, IAttributeFacade attributeFacade) : base(address, password, new Web3())
        {
            _factoryAddress = factoryAddress;
            _attributeFacade = attributeFacade;
        }

        public IDFacade(string address, string password, IAttributeFacade attributeFacade) : base(address, password, new Web3())
        {
            _attributeFacade = attributeFacade;
        }

        public async Task<ID> DeployAsync(ID id)
        {
            FactoryService factory = new FactoryService(_web3, _factoryAddress);
            Event idCreationEvent = factory.GetEventReturnIDController();
            HexBigInteger filterAddressFrom = await idCreationEvent.CreateFilterAsync(_address);
            await factory.CreateIDAsync(_address, new HexBigInteger(3905820));

            var log = await idCreationEvent.GetFilterChanges<ReturnIDControllerEventDTO>(filterAddressFrom);

            string controllerAddress = log[0].Event._controllerAddress;

            id.Address = controllerAddress;

            Dictionary<string, Attribute> updatedAttributes = new Dictionary<string, Attribute>();

            //Add each attribute from the ID model to the ID smart contract
            foreach (string key in id.Attributes.Keys)
            {
                Attribute attribute = id.GetAttribute(key);

                attribute = await AddAttributeAsync(id, Encoding.UTF8.GetBytes(key), attribute);
                updatedAttributes.Add(key, attribute);
            }
            id.Attributes = updatedAttributes;
            return id;
        }

        public async Task<ID> GetIDAsync(string address)
        {
            ID newID = new ID();
            newID.Address = address;

            //Get all attributes from the smart contract and add them to the ID model
            Dictionary<byte[], Attribute> attributes = await GetAttributesAsync(newID);
            foreach (byte[] key in attributes.Keys)
            {
                string keyStr = Encoding.UTF8.GetString(key, 0, key.Length);
                newID.AddAttribute(keyStr, attributes[key]);
            }

            return newID;

        }

        public async Task<Attribute> AddAttributeAsync(ID id, byte[] key, Attribute attribute)
        {
            IDControllerService ethIDCtrl = new IDControllerService(_web3, id.Address);

            //If the attribute to be added is not yet deployed, deploy it
            if (attribute.Address == null)
                attribute = await _attributeFacade.DeployAsync(attribute, id);

            await ethIDCtrl.AddAttributeAsync(_address, key, attribute.Address, new HexBigInteger(3905820));

            return attribute;
        }

        public async Task<Dictionary<byte[], Attribute>> GetAttributesAsync(ID id)
        {
            IDControllerService ethIdCtrl = new IDControllerService(_web3, id.Address);
            Dictionary<byte[], Attribute> dict = new Dictionary<byte[], Attribute>();

            BigInteger attributes = await ethIdCtrl.AttributeCountAsyncCall();
            for (BigInteger i = 0; i < attributes; i++)
            {
                //Get all attribute keys and addresses for the ID
                byte[] attributeKey = await ethIdCtrl.GetAttributeKeyAsyncCall(i);
                string ethAttributeAddress = await ethIdCtrl.GetAttributeAsyncCall(attributeKey);
                //Get the attribute and add it to the local ID model
                Attribute newAttribute = await _attributeFacade.GetAttributeAsync(ethAttributeAddress);
                dict.Add(attributeKey, newAttribute);
            }

            return dict;
        }

    }
}
