using System;
using Lynx.Core.Communications.Packets;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eVi.abi.lib.pcl;
using Lynx.Core.Facade;
using Lynx.Core.Facade.Interfaces;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.ABI.Encoders;
using Lynx.Core.Models.IDSubsystem;
using Lynx.Core;
using Lynx.Core.Communications.Packets.Interfaces;
using Lynx.Core.Crypto;
using Lynx.Core.Crypto.Interfaces;
using Lynx.Core.Interfaces;
using Lynx.Core.PeerVerification;
using Nethereum.Web3;
using Attribute = Lynx.Core.Models.IDSubsystem.Attribute;

namespace CoreUnitTests.PCL
{
    public class TokenFactoryTest
    {
        private readonly string _privateKey1 = "9e6a6bf412ce4e3a91a33c7c0f6d94b3127b8d4f5ed336210a672fe595bf1769";
		private IIDFacade _idFacade;
        private IAccountService _accountService;
        private TokenFactory<Syn> _tokenFactory;

		[SetUp]
		public void Setup()
		{
			_accountService = new AccountService(_privateKey1);
			_idFacade = new DummyIDFacade(_accountService);
            _tokenFactory = new TokenFactory<Syn>(_idFacade);

		}

        [Test]
        public async Task TestCreateToken()
        {
            string encodedToken = "header.payload";
            Syn syn = await _tokenFactory.CreateToken(encodedToken);

        }

        /// <summary>
        /// Dummy IDFacade that return a fully formed dummy ID
        /// </summary>
        private class DummyIDFacade : IIDFacade
        {
            private IAccountService _accountService;

            public DummyIDFacade(IAccountService accountService)
            {
                _accountService = accountService;
            }

            public Task<Attribute> AddAttributeAsync(ID id, byte[] key, Attribute attribute)
            {
                throw new NotImplementedException();
            }

            public Task<ID> DeployAsync(ID id)
            {
                throw new NotImplementedException();
            }

            public Task<Dictionary<string, Attribute>> GetAttributesAsync(ID id)
            {
                throw new NotImplementedException();
            }

            public Task<Dictionary<string, Attribute>> GetAttributesAsync(ID id, string[] accessibleAttributes)
            {
                throw new NotImplementedException();   
            }

            public async Task<ID> GetIDAsync(string address, string[] accessibleAttributes)
            {
                ID id = new ID();
                foreach (string key in accessibleAttributes)
                {
                    Attribute attr = new Attribute()
                    {
                        Description = key,
                        Location = "Location of " + key,
                        Hash = "Hash of" + key,
                        Content = new StringContent(key + " content")
                    };

                    id.AddAttribute(attr.Description, attr);
                }
                id.Address = address;
                id.Owner = _accountService.GetAccountAddress();

                return id;
            }
        }

    }
}
