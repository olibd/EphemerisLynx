using System;
using System.Threading.Tasks;
using Lynx.Core;
using Lynx.Core.Communications.Packets;
using Lynx.Core.Communications.Packets.Interfaces;
using Lynx.Core.Facade.Interfaces;
using Lynx.Core.Models.IDSubsystem;
using NUnit.Framework;
using static CoreUnitTests.PCL.RequesterVerifierTest;

namespace CoreUnitTests.PCL
{
    public class HandshakeTokenFactoryTest
    {
        private IHandshakeToken _token;
        private AccountService _account;
        private IIDFacade _idFacade;
        private HandshakeTokenFactory<Syn> _hsTokenFactory;
        private ID _id;
        private IHandshakeToken _restoredSyn;

        [SetUp]
        public void Setup()
        {
            _account = new AccountService("9e6a6bf412ce4e3a91a33c7c0f6d94b3127b8d4f5ed336210a672fe595bf1769");
            _idFacade = new DummyIDFacade(_account);

            SetupIDsAsync().Wait();

            _token = new Syn()
            {
                Encrypted = false,
                PublicKey = _account.PublicKey,
                NetworkAddress = "a8aa460b582f9dd543835d2388dd7b0b15fa0ddfbc5d386cf187df6720e3d95656789abcd56789abcd56789abcd",
                Id = _id
            };

            _token.SignAndLock("sig");
            _restoredSyn = null;
            _hsTokenFactory = new HandshakeTokenFactory<Syn>(_idFacade);
        }

        public async Task SetupIDsAsync()
        {
            _id = await _idFacade.GetIDAsync("0x1FD8397e8108ada12eC07976D92F773364ba46e7", new string[] { "firstname", "lastname", "cell", "address", "extra" });
        }

        [Test]
        public void CreateHandshakeTokenAsyncTest()
        {
            //Create an ecoded version of the token
            string encodedToken = _token.GetEncodedToken();

            //instantiate a new instance of the same token using the facotry and the encoded token
            RestoreSynAsync(encodedToken).Wait();

            //Compare the new token to the old token
            Assert.AreEqual(encodedToken, _restoredSyn.GetEncodedToken());
            Assert.NotNull(_restoredSyn.Id);
            Assert.AreEqual(_token.Encrypted, _restoredSyn.Encrypted);
            Assert.AreEqual(_token.AccessibleAttributes, _restoredSyn.AccessibleAttributes);
            Assert.AreEqual(_token.PublicKey, _restoredSyn.PublicKey);
            Assert.AreEqual(_token.Signature, _restoredSyn.Signature);
        }

        public async Task RestoreSynAsync(string encodedToken)
        {
            _restoredSyn = await _hsTokenFactory.CreateHandshakeTokenAsync(encodedToken);
        }
    }
}
