using System;
using Lynx.Core;
using NUnit.Framework;

namespace CoreUnitTests.PCL
{
    [TestFixture()]
    public class AccountServiceTest
    {
        private AccountService _accountService;
        private string _privateKey;
        private string _publicKey;
        private byte[] _privKeyByteArr;
        private byte[] _pubKeyByteArr;
        private string _publicAddress;

        [SetUp]
        public void Setup()
        {
            _privateKey = "9e6a6bf412ce4e3a91a33c7c0f6d94b3127b8d4f5ed336210a672fe595bf1769";
            _publicKey = "433bd128c7ee7677ffe9482e30c2c6f02a79b04fb3c562e323c755e5f16699b9342b031bb1107f75ec12203a908d65fac6629920b1f69a72990ce6aa12ba4cce0";
            _privKeyByteArr = new byte[32] { 158, 106, 107, 244, 18, 206, 78, 58,
                145, 163, 60, 124, 15, 109, 148, 179, 18, 123, 141, 79, 94, 211,
                54, 33, 10, 103, 47, 229, 149, 191, 23, 105 };
            _pubKeyByteArr = new byte[65] { 4, 51, 189, 18, 140, 126, 231, 103,
                127, 254, 148, 130, 227, 12, 44, 111, 2, 167, 155, 4, 251, 60,
                86, 46, 50, 60, 117, 94, 95, 22, 105, 155, 147, 66, 176, 49,
                187, 17, 7, 247, 94, 193, 34, 3, 169, 8, 214, 95, 172, 102,
                41, 146, 11, 31, 105, 167, 41, 144, 206, 106, 161, 43, 164,
                204, 224 };
            _publicAddress = "0x68004661F764c169805C8B6834cab7A65D448c74";
            _accountService = new AccountService(_privateKey);
        }

        [Test]
        public void TestPublicKeyAsString()
        {
            Assert.AreEqual(_publicKey, _accountService.PublicKey);
        }

        [Test]
        public void TestPublicKeyAsByteArray()
        {
            Assert.AreEqual(_pubKeyByteArr, _accountService.GetPublicKeyAsByteArray());
        }

        [Test]
        public void TestPrivateKeyAsByteArray()
        {
            Assert.AreEqual(_privKeyByteArr, _accountService.GetPrivateKeyAsByteArray());
        }

        [Test]
        public void TestAccountAddress()
        {
            Assert.AreEqual(_publicAddress.ToLower(), _accountService.GetAccountAddress());
        }
    }
}
