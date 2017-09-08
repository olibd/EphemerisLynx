using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lynx.Core;
using Lynx.Core.Communications.Packets;
using Lynx.Core.Communications.Packets.Interfaces;
using Lynx.Core.Crypto;
using Lynx.Core.Crypto.Interfaces;
using Lynx.Core.Facade;
using Lynx.Core.Facade.Interfaces;
using Lynx.Core.Models.IDSubsystem;
using Lynx.Core.PeerVerification;
using Nethereum.Web3;
using Newtonsoft.Json;
using NUnit.Framework;
using Attribute = Lynx.Core.Models.IDSubsystem.Attribute;

namespace CoreUnitTests.PCL
{
    [TestFixture()]
    public class RequesterTest
    {
        private Requester _requester;
        private ID _id;
        private AccountService _accountService;
        private string _privateKey;
        private ITokenCryptoService<IToken> _tokenCryptoService;
        private IIDFacade _idFacade;
        private ICertificateFacade _certFacade;
        private IAttributeFacade _attributeFacade;
        protected Web3 _web3;
        protected string _addressFrom;

        [SetUp]
        public void Setup()
        {
            /////////////////////
            //Create a dummy ID//
            /////////////////////

            //create some dummy attributes
            Attribute firstname = new Attribute()
            {
                Location = "1",
                Hash = "1",
                Content = new StringContent("Olivier")
            };

            Attribute lastname = new Attribute()
            {
                Location = "2",
                Hash = "2",
                Content = new StringContent("Brochu Dufour")
            };

            Attribute age = new Attribute()
            {
                Location = "3",
                Hash = "3",
                Content = new IntContent(24)
            };

            Attribute cell = new Attribute()
            {
                Location = "4",
                Hash = "4",
                Content = new StringContent("555-555-5555")
            };

            Attribute address = new Attribute()
            {
                Location = "5",
                Hash = "5",
                Content = new StringContent("1 infinite loop, cupertino")
            };

            _id = new ID();
            _id.AddAttribute("firstname", firstname);
            _id.AddAttribute("lastname", lastname);
            _id.AddAttribute("age", age);
            _id.AddAttribute("cell", cell);
            _id.AddAttribute("address", address);
            _id.Address = "0x1234567";

            ////////////////////////
            //Create the ID Facade//
            ////////////////////////
            initWeb3().Wait();
            _certFacade = new CertificateFacade(_addressFrom, "", _web3, new DummyContentService());
            _attributeFacade = new AttributeFacade(_addressFrom, "", _web3, _certFacade, new DummyContentService());
            _idFacade = new IDFacade(_addressFrom, "", "0x0", _web3, _attributeFacade);

            /////////////////////////
            //Create an eth account//
            /////////////////////////
            _privateKey = "9e6a6bf412ce4e3a91a33c7c0f6d94b3127b8d4f5ed336210a672fe595bf1769";
            _accountService = new AccountService(_privateKey);
            _tokenCryptoService = new TokenCryptoService<IToken>(new SECP256K1CryptoService());
            _requester = new Requester(_tokenCryptoService, _accountService, _id, _idFacade, _attributeFacade, _certFacade);
        }

        [Test]
        public void CreateEncodedSynTest()
        {
            //check the type
            string encodedSyn = _requester.CreateEncodedSyn();
            Assert.True(encodedSyn.Contains(":"));

            //remove the type
            string untypedEncodedToken = encodedSyn.Split(':')[1];

            string[] splittedEncodedToken = untypedEncodedToken.Split('.');

            Assert.AreEqual(3, splittedEncodedToken.Length);

            string jsonDecodedHeader = Base64Decode(splittedEncodedToken[0]);
            string jsonDecodedPayload = Base64Decode(splittedEncodedToken[1]);

            try
            {
                Dictionary<string, string> header = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonDecodedHeader);
                Dictionary<string, string> payload = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonDecodedPayload);

                Assert.True(header.ContainsKey("encrypted"));
                Assert.AreEqual("False", header["encrypted"]);

                Assert.True(payload.ContainsKey("idAddr"));
                Assert.AreEqual("0x1234567", payload["idAddr"]);

                Assert.True(header.ContainsKey("pubkey"));
                Assert.AreEqual(_accountService.PublicKey, header["pubkey"]);

                //Testing only the presence of the key and not
                //the value because the value is generated on
                //the fly
                Assert.True(payload.ContainsKey("netAddr"));


            }
            catch (Exception e)
            {
                Assert.Fail("Failed to convert json to dictionaries. The returned token is invalid: " + e);
            }
        }

        public string Base64Decode(string encodedText)
        {
            byte[] plainTextBytes = Convert.FromBase64String(encodedText);
            return System.Text.Encoding.UTF8.GetString(plainTextBytes, 0, plainTextBytes.Length);
        }

        private async Task initWeb3()
        {
            _web3 = new Web3("http://10.0.1.11:8082");
            _addressFrom = (await _web3.Eth.Accounts.SendRequestAsync())[0];
        }
    }
}
