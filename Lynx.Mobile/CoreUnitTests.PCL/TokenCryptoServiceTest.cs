using System;
using Lynx.Core;
using Lynx.Core.Communications.Packets;
using Lynx.Core.Communications.Packets.Interfaces;
using Lynx.Core.Crypto;
using Lynx.Core.Models.IDSubsystem;
using NUnit.Framework;


namespace CoreUnitTests.PCL
{
    [TestFixture()]
    public class TokenCryptoServiceTest
    {
        private TokenCryptoService<IToken> _tCS;
        private AccountService _account;
        private AccountService _account2;
        private IToken token;

        [SetUp]
        public void Setup()
        {
            _tCS = new TokenCryptoService<IToken>(new SECP256K1CryptoService());

            _account = new AccountService("9e6a6bf412ce4e3a91a33c7c0f6d94b3127b8d4f5ed336210a672fe595bf1769");
            _account2 = new AccountService("cbbeecc0d2d9ec5991733fc168f6908fda9613cf37c95e8e524e3a62b5d7b161");

            ID id = new ID()
            {
                ControllerAddress = "56789abcd"
            };

            token = new Syn()
            {
                Encrypted = false,
                PublicKey = _account.PublicKey,
                NetworkAddress = "123",
                Id = id
            };
        }

        [Test]
        public void SignVerifyTest()
        {
            //Sign
            Assert.Null(token.Signature);
            _tCS.Sign(token, _account.GetPrivateKeyAsByteArray());
            Assert.NotNull(token.Signature);

            //Verify: Positive Scenario
            Assert.IsTrue(_tCS.Verify(token, _account.GetPublicKeyAsByteArray()));

            //Verify: Negative Scenario
            Assert.IsFalse(_tCS.Verify(token, _account2.GetPublicKeyAsByteArray()));
        }

        [Test]
        public void TestEncryptAndDecrypt()
        {
            string cipherData = _tCS.Encrypt(token, _account.GetPublicKeyAsByteArray(), _account.GetPrivateKeyAsByteArray());
            Assert.AreNotEqual(null, cipherData);
            string decryptedData = _tCS.Decrypt(cipherData, _account.GetPrivateKeyAsByteArray());
            Assert.AreNotEqual(cipherData, decryptedData);
            string[] splittedDecryptedData = decryptedData.Split('.');
            Assert.AreEqual(token.GetEncodedHeader(), splittedDecryptedData[0]);
            Assert.AreEqual(token.GetEncodedPayload(), splittedDecryptedData[1]);
        }
    }
}
