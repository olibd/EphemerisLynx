using Lynx.Core.Facade.Interfaces;
using System;
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
    class IDFacade<T> : Facade, IIDFacade<T>
    {
        string _factoryAddress;

        public IDFacade(string address, string password, string factoryAddress, Web3 web3) : base(address, password, web3)
        {
            _factoryAddress = factoryAddress;
        }

        public IDFacade(string address, string password, string factoryAddress) : base(address, password, new Web3())
        {
            _factoryAddress = factoryAddress;
        }

        public IDFacade(string address, string password) : base(address, password, new Web3())
        {
        }

        public async Task<ID> DeployAsync(ID id)
        {
            ID newID = new ID();
            string idAddress = await IDService.DeployContractAsync(_web3, _address);

            newID.Address = idAddress;

            //Add each attribute from the ID model to the ID smart contract
            IAttributeFacade<T> ethAttribute = new AttributeFacade<T>(_address, _password, _web3);
            foreach (string key in id.GetAttributeKeys())
            {
                Attribute<T> attribute = id.GetAttribute<T>(key);

                //Should only happen if the attribute does not match the desired type 
                if (attribute == null) continue;

                attribute = await AddAttributeAsync(newID, Encoding.UTF8.GetBytes(key), attribute);
                newID.AddAttribute(attribute);
            }
            return id;
        }

        public async Task<ID> GetIDAsync(string address)
        {
            ID newID = new ID();
            IDService ethId = new IDService(_web3, address);
            newID.Address = address;

            //Get all attributes from the smart contract and add them to the ID model
            Dictionary<byte[], Attribute<T>> attributes = await GetAttributesAsync(newID);
            foreach (Attribute<T> attr in attributes.Values)
            {
                newID.AddAttribute(attr);
            }

            return newID;

        }

        public async Task<Attribute<T>> AddAttributeAsync(ID id, byte[] key, Attribute<T> attribute)
        {
            IDService ethID = new IDService(_web3, id.Address);
            IAttributeFacade<T> AttributeFacade = new AttributeFacade<T>(_address, _password, _web3);

            //If the attribute to be added is not yet deployed, deploy it
            if (attribute.Address == "")
                attribute = await AttributeFacade.DeployAsync(attribute);

            await ethID.AddAttributeAsync(_address, key, attribute.Address);

            return attribute;
        }

        public async Task<Dictionary<byte[], Attribute<T>>> GetAttributesAsync(ID id)
        {
            IDService ethId = new IDService(_web3, id.Address);
            Dictionary<byte[], Attribute<T>> dict = new Dictionary<byte[], Attribute<T>>();

            BigInteger attributes = await ethId.AttributeCountAsyncCall();
            for (BigInteger i = 0; i < attributes; i++)
            {
                //Get all attribute keys and addresses for the ID
                byte[] attributeKey = await ethId.AttributesKeysAsyncCall(i);
                string ethAttributeAddress = await ethId.AttributesAsyncCall(attributeKey);
                AttributeFacade<T> attributeFacade = new AttributeFacade<T>(_address, _password, _web3);
                //Get the attribute and add it to the local ID model
                Attribute<T> newAttribute = await attributeFacade.GetAttributeAsync(ethAttributeAddress);
                dict.Add(attributeKey, newAttribute);
            }

            return dict;
        }

    }
}
