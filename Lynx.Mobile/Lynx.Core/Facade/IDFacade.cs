﻿using System;
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
using Lynx.Core.Interfaces;
using Nethereum.ABI.Encoders;
using Attribute = Lynx.Core.Models.IDSubsystem.Attribute;

namespace Lynx.Core.Facade
{
    public class IDFacade : Facade, IIDFacade
    {
        private string _factoryAddress;
        private IAttributeFacade _attributeFacade;

        public IDFacade(string factoryAddress, Web3 web3, IAttributeFacade attributeFacade, IAccountService accountService) : base(web3, accountService)
        {
            _factoryAddress = factoryAddress;
            _attributeFacade = attributeFacade;
        }

        public IDFacade(string factoryAddress, IAttributeFacade attributeFacade, IAccountService accountService) : base(new Web3(), accountService)
        {
            _factoryAddress = factoryAddress;
            _attributeFacade = attributeFacade;
        }

        public IDFacade(IAttributeFacade attributeFacade, IAccountService accountService) : base(new Web3(), accountService)
        {
            _attributeFacade = attributeFacade;
        }

        public async Task<ID> DeployAsync(ID id)
        {
            FactoryService factory = new FactoryService(Web3, AccountService.PrivateKey, _factoryAddress);
            Bytes32TypeEncoder encoder = new Bytes32TypeEncoder();

            //Use the provided Factory address to create an ID + IDController
            Event idCreationEvent = factory.GetEventReturnIDController();
            HexBigInteger filterAddressFrom =
                await idCreationEvent.CreateFilterAsync();
            await factory.CreateIDAsync();

            List<EventLog<ReturnIDControllerEventDTO>> log =
                await idCreationEvent.GetFilterChanges<ReturnIDControllerEventDTO>(filterAddressFrom);

            string controllerAddress = log[0].Event._controllerAddress;
            IDControllerService idcService =
                new IDControllerService(Web3, AccountService.PrivateKey, controllerAddress);

            id.ControllerAddress = controllerAddress;
            id.Address = await idcService.GetIDAsyncCall();


            //Add each attribute from the ID model to the ID smart contract
            foreach (string key in id.Attributes.Keys)
            {
                Attribute attribute = id.GetAttribute(key);
                await AddAttributeAsync(id, encoder.Encode(key), attribute);
            }
            return id;
        }

        //This function will only be used to create the initial ID object on login
        public async Task<ID> GetIDAsync(string address)
        {
            IDControllerService idcService = new IDControllerService(Web3, AccountService.PrivateKey, address);

            ID newID = new ID
            {
                ControllerAddress = address,
                Address = await idcService.GetIDAsyncCall()
            };

            //Get all attributes from the smart contract and add them to the ID object
            Dictionary<string, Attribute> attributes = await GetAttributesAsync(newID);
            foreach (string key in attributes.Keys)
            {
                newID.AddAttribute(key, attributes[key]);
            }

            return newID;
        }

        public async Task<Attribute> AddAttributeAsync(ID id, byte[] key, Attribute attribute)
        {
            IDControllerService idcService = new IDControllerService(Web3, AccountService.PrivateKey, id.ControllerAddress);

            //If the attribute to be added is not yet deployed, deploy it
            if (attribute.Address == null)
                attribute = await _attributeFacade.DeployAsync(attribute, id.Address);


            await idcService.AddAttributeAsync(key, attribute.Address);

            return attribute;
        }

        public async Task<Dictionary<string, Attribute>> GetAttributesAsync(ID id)
        {
            IDControllerService idcService = new IDControllerService(Web3, AccountService.PrivateKey, id.ControllerAddress);
            Dictionary<string, Attribute> dict = new Dictionary<string, Attribute>();

            BigInteger attributes = await idcService.AttributeCountAsyncCall();
            for (BigInteger i = 0; i < attributes; i++)
            {
                //Get all attribute keys and addresses for the ID
                byte[] attributeKey = await idcService.GetAttributeKeyAsyncCall(i);

                string ethAttributeAddress = await idcService.GetAttributeAsyncCall(attributeKey);
                //Get the attribute and add it to the dict
                Attribute newAttribute = await _attributeFacade.GetAttributeAsync(ethAttributeAddress);
                string keyStr = Encoding.UTF8.GetString(attributeKey, 0, attributeKey.Length);
                dict.Add(keyStr, newAttribute);
            }

            return dict;
        }

    }
}
