using System;
using Lynx.Core.Models.IDSubsystem;
using Lynx.Core.Services;
using Lynx.Core.Services.Interfaces;
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

            /////////////////////////
            //Create an eth account//
            /////////////////////////
            _privateKey = "9e6a6bf412ce4e3a91a33c7c0f6d94b3127b8d4f5ed336210a672fe595bf1769";
            _accountService = new AccountService(_privateKey);
            _tokenCryptoService = new TokenCryptoService<IHandshakeToken>(new SECP256K1CryptoService());
            _requesterService = new RequesterService(_tokenCryptoService, _accountService, _id);
        }

        [Test]
        public void CreateEncodedSynTest()
        {
            string originalEncodedSyn = _requesterService.CreateEncodedSyn();
            Syn decodedSyn = new Syn(originalEncodedSyn);
            string reencodedSyn = decodedSyn.GetEncodedToken();
            Assert.AreEqual(originalEncodedSyn, reencodedSyn);
        }
    }
}
