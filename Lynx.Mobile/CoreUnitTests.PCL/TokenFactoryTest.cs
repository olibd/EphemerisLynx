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
    public class TokenFactoryTest
    {
        private IToken _token;
        private AccountService _account;
        private TokenFactory<Syn> _tokenFactory;
        private IToken _restoredSyn;

        [SetUp]
        public virtual void Setup()
        {
            _account = new AccountService("9e6a6bf412ce4e3a91a33c7c0f6d94b3127b8d4f5ed336210a672fe595bf1769");

            _token = new Syn()
            {
                Encrypted = false,
                PublicKey = _account.PublicKey,
                NetworkAddress = "a8aa460b582f9dd543835d2388dd7b0b15fa0ddfbc5d386cf187df6720e3d95656789abcd56789abcd56789abcd",
            };

            _token.SignAndLock("sig");
            _restoredSyn = null;
            _tokenFactory = new TokenFactory<Syn>();
        }

        [Test]
        public void CreateHandshakeTokenAsyncTest()
        {
            string encodedToken = _token.GetEncodedToken();
            _restoredSyn = _tokenFactory.CreateToken(encodedToken);
            Assert.AreEqual(encodedToken, _restoredSyn.GetEncodedToken());
            Assert.AreEqual(_token.PublicKey, _restoredSyn.PublicKey);
            Assert.AreEqual(_token.Signature, _restoredSyn.Signature);
        }
    }
}
