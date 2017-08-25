using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using eVi.abi.lib.pcl;
using Lynx.Core;
using Lynx.Core.Communications.Packets;
using Lynx.Core.Communications.Packets.Interfaces;
using Lynx.Core.Crypto;
using Lynx.Core.Crypto.Interfaces;
using Lynx.Core.Facade;
using Lynx.Core.Facade.Interfaces;
using Lynx.Core.Interfaces;
using Lynx.Core.Models.IDSubsystem;
using Lynx.Core.PeerVerification;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using NUnit.Framework;
using Attribute = Lynx.Core.Models.IDSubsystem.Attribute;

namespace CoreUnitTests.PCL
{
    public class RequesterVerifierTest
    {
        private readonly string _privateKey1 = "9e6a6bf412ce4e3a91a33c7c0f6d94b3127b8d4f5ed336210a672fe595bf1769";
        private readonly string _privateKey2 = "b233d69dfc622ea94821f89044f5b3a82b0464a6a25177a5e39f11cfa8992009";
        private ITokenCryptoService<IHandshakeToken> _tkCrypto;
        private IAccountService _accountService1;
        private IAccountService _accountService2;
        private IIDFacade _idFacade1;
        private IIDFacade _idFacade2;
        private RequesterService _requester;
        private Verifier _verifier;
        private ID _id1;
        private ID _id2;
        private SynAck _synAck;
        private readonly string[] _allAttributes = { "firstname", "lastname", "cell", "address", "extra" };
        private readonly string[] _accessibleAttributes = { "firstname", "lastname", "cell", "address" };
        private readonly string idAddr1 = "0x1FD8397e8108ada12eC07976D92F773364ba46e7";
        private readonly string idAddr2 = "0x00a329c0648769A73afAc7F9381E08FB43dBEA72";

        [SetUp]
        public void Setup()
        {
            _tkCrypto = new TokenCryptoService<IHandshakeToken>(new SECP256K1CryptoService());
            _accountService1 = new AccountService(_privateKey1);
            _accountService2 = new AccountService(_privateKey2);

            _idFacade1 = new DummyIDFacade(_accountService1);
            _idFacade2 = new DummyIDFacade(_accountService2);


            SetupIDsAsync().Wait();

            //Uses ID facade 2 because it wants to be able to load ID 2
            _requester = new RequesterService(_tkCrypto, _accountService1, _id1, _idFacade2);
            //Uses ID facade 1 because it wants to be able to load ID 1
            _verifier = new Verifier(_tkCrypto, _accountService2, _id2, _idFacade1);
        }

        public async Task SetupIDsAsync()
        {
            _id1 = await _idFacade1.GetIDAsync(idAddr1, new string[] { "firstname", "lastname", "cell", "address", "extra" });
            _id2 = await _idFacade2.GetIDAsync(idAddr2, new string[] { "firstname", "lastname", "cell", "address", "extra2" });
        }

        [Test]
        public void VerificationRequestHandshakeTest()
        {
            string encodedSyn = _requester.CreateEncodedSyn();
            ManualResetEvent waitHandle = new ManualResetEvent(false);
            _verifier.IdentityProfileReceived += (sender, e) =>
            {
                _synAck = e.SynAck;
                waitHandle.Set();
            };

            _verifier.ProcessSyn(encodedSyn).Wait();

            if (waitHandle.WaitOne(100000))
            {
                foreach (string attrKey in _accessibleAttributes)
                {
                    Assert.IsNotNull(_synAck.Id.GetAttribute(attrKey));
                }

                //Check if the extra attribute is present. It should not be
                //because it is not part of the accessible attributes arrray

                bool failed = false;
                try
                {
                    _synAck.Id.GetAttribute(_allAttributes[4]);
                }
                catch (Exception e)
                {
                    failed = true;
                }
                Assert.True(failed);
            }
            else
                Assert.Fail();
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

            public async Task<ID> GetIDAsync(string address, string[] accessibleAttributes)
            {
                ID id = new ID();

                if (accessibleAttributes == null)
                    accessibleAttributes = new string[] { "firstname", "lastname", "cell", "address", "extra" };

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

            public Task<Dictionary<string, Attribute>> GetAttributesAsync(ID id, string[] accessibleAttributes)
            {
                throw new NotImplementedException();
            }
        }
    }
}
