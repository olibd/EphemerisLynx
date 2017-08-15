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
    public class RequesterServiceTest
    {
        private RequesterService _requesterService;
        private ID _id;
        private AccountService _accountService;
        private string _privateKey;
        private ITokenCryptoService<IHandshakeToken> _tokenCryptoService;
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

            _id = new ID();
            _id.AddAttribute("Firstname", firstname);
            _id.AddAttribute("Lastname", lastname);
            _id.AddAttribute("Age", age);

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
            _tokenCryptoService = new TokenCryptoService<IHandshakeToken>(new SECP256K1CryptoService());
            _requesterService = new RequesterService(_tokenCryptoService, _accountService, _id, _idFacade);
        }

        [Test]
        public void CreateEncodedSynTest()
        {
            string encodedToken = _requesterService.CreateEncodedSyn();

            string[] splittedEncodedToken = encodedToken.Split('.');

            Assert.AreEqual(3, splittedEncodedToken);

            string jsonDecodedHeader = Base64Decode(splittedEncodedToken[0]);
            string jsonDecodedPayload = Base64Decode(splittedEncodedToken[1]);

            try
            {
                Dictionary<string, string> header = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonDecodedHeader);
                Dictionary<string, string> payload = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonDecodedPayload);
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
