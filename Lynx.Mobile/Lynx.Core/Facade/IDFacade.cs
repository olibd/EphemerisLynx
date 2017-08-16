﻿using Lynx.Core.Facade.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nethereum.Web3;
using Lynx.Core.Models.IDSubsystem;
using eVi.abi.lib.pcl;
using Nethereum.Hex.HexTypes;
using System.Numerics;
using Nethereum.ABI.Encoders;

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
            FactoryService factory = new FactoryService(Web3, _factoryAddress);
            Bytes32TypeEncoder encoder = new Bytes32TypeEncoder();

            //Use the provided Factory address to create an ID + IDController
            Event idCreationEvent = factory.GetEventReturnIDController();
            HexBigInteger filterAddressFrom = await idCreationEvent.CreateFilterAsync(Address);
            await factory.CreateIDAsync(Address, new HexBigInteger(3905820));

            List<EventLog<ReturnIDControllerEventDTO>> log = await idCreationEvent.GetFilterChanges<ReturnIDControllerEventDTO>(filterAddressFrom);

            string controllerAddress = log[0].Event._controllerAddress;
            IDControllerService idcService = new IDControllerService(Web3, controllerAddress);

            id.ControllerAddress = controllerAddress;
            id.Address = await idcService.GetIDAsyncCall();
            id.Owner = await idcService.OwnerAsyncCall();


            //Add each attribute from the ID model to the ID smart contract
            foreach (string key in id.Attributes.Keys)
            {
                Attribute attribute = id.GetAttribute(key);
                await AddAttributeAsync(id, encoder.Encode(key), attribute);
            }
            return id;
        }

		//This function will only be used to create the initial ID object on login
		public async Task<ID> GetIDAsync(string address, string[] accessibleAttributes = null)
		{
			IDControllerService idcService = new IDControllerService(Web3, address);

			ID newID = new ID
			{
				ControllerAddress = address,
				Address = await idcService.GetIDAsyncCall()
			};
            //Get attributes from the smart contract and add them to the ID object
            Dictionary<string, Attribute> attributes;
            if (accessibleAttributes != null)
                attributes = await GetAttributesAsync(newID, accessibleAttributes);
            else
			    attributes = await GetAttributesAsync(newID);
			foreach (string key in attributes.Keys)
			{
				newID.AddAttribute(key, attributes[key]);
			}

			return newID;
		}

        public async Task<Attribute> AddAttributeAsync(ID id, byte[] key, Attribute attribute)
        {
            IDControllerService idcService = new IDControllerService(Web3, id.ControllerAddress);

            //If the attribute to be added is not yet deployed, deploy it
            if (attribute.Address == null)
                attribute = await _attributeFacade.DeployAsync(attribute, id.Address);

            await idcService.AddAttributeAsync(Address, key, attribute.Address, new HexBigInteger(3905820));

            return attribute;
        }

        public async Task<Dictionary<string, Attribute>> GetAttributesAsync(ID id)
        {
            IDControllerService idcService = new IDControllerService(Web3, id.ControllerAddress);
            Dictionary<string, Attribute> dict = new Dictionary<string, Attribute>();

            BigInteger attributes = await idcService.AttributeCountAsyncCall();
            for (BigInteger i = 0; i < attributes; i++)
            {
                //Get all attribute keys and addresses for the ID
                byte[] attributeKey = await idcService.GetAttributeKeyAsyncCall(i);
				//Get the attribute and add it to the dict
				Attribute newAttribute = await GetAttributeByKey(idcService, attributeKey);
                string keyStr = Encoding.UTF8.GetString(attributeKey, 0, attributeKey.Length);
                dict.Add(keyStr, newAttribute);
            }

            return dict;
        }

		public async Task<Dictionary<string, Attribute>> GetAttributesAsync(ID id, string[] accessibleAttributes)
		{
			IDControllerService idcService = new IDControllerService(Web3, id.ControllerAddress);
			Dictionary<string, Attribute> dict = new Dictionary<string, Attribute>();

			BigInteger attributes = await idcService.AttributeCountAsyncCall();
			for (BigInteger i = 0; i < attributes; i++)
			{
				//Get all attribute keys and addresses for the ID
				byte[] attributeKey = await idcService.GetAttributeKeyAsyncCall(i);
                string keyStr = Encoding.UTF8.GetString(attributeKey, 0, attributeKey.Length);
                if (accessibleAttributes.Contains(keyStr))
				{
					//Get the attribute and add it to the dict
					Attribute newAttribute = await GetAttributeByKey(idcService, attributeKey);
					dict.Add(keyStr, newAttribute);
                }
			}
			return dict;
		}

        private async Task<Attribute> GetAttributeByKey(IDControllerService idcService, byte[] attributeKey) 
        {
			string ethAttributeAddress = await idcService.GetAttributeAsyncCall(attributeKey);
			//Get the attribute and add it to the dict
			Attribute attribute = await _attributeFacade.GetAttributeAsync(ethAttributeAddress);
            return attribute; 
        }

    }
}
