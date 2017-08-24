﻿using System;
using Lynx.Core;
using Lynx.Core.Crypto;
using Lynx.Core.Interfaces;
using NUnit.Framework;
using Org.BouncyCastle.Crypto.Parameters;


namespace CoreUnitTests.PCL
{
	public class SECP256K1CryptoServiceTest
	{
		private SECP256K1CryptoService _cryptoService = new SECP256K1CryptoService();
		private byte[] pubkey = {
			0x04,
			0x82, 0x00, 0x6e, 0x93, 0x98, 0xa6, 0x98, 0x6e,
			0xda, 0x61, 0xfe, 0x91, 0x67, 0x4c, 0x3a, 0x10,
			0x8c, 0x39, 0x94, 0x75, 0xbf, 0x1e, 0x73, 0x8f,
			0x19, 0xdf, 0xc2, 0xdb, 0x11, 0xdb, 0x1d, 0x28,
			0x13, 0x0c, 0x6b, 0x3b, 0x28, 0xae, 0xf9, 0xa9,
			0xc7, 0xe7, 0x14, 0x3d, 0xac, 0x6c, 0xf1, 0x2c,
			0x09, 0xb8, 0x44, 0x4d, 0xb6, 0x16, 0x79, 0xab,
			0xb1, 0xd8, 0x6f, 0x85, 0xc0, 0x38, 0xa5, 0x8c
		};
		static private byte[] privkey = {
		   0x16, 0x26, 0x07, 0x83, 0xe4, 0x0b, 0x16, 0x73,
		   0x16, 0x73, 0x62, 0x2a, 0xc8, 0xa5, 0xb0, 0x45,
		   0xfc, 0x3e, 0xa4, 0xaf, 0x70, 0xf7, 0x27, 0xf3,
		   0xf9, 0xe9, 0x2b, 0xdd, 0x3a, 0x1d, 0xdc, 0x42
		};
		private byte[] data = {
		   0x16, 0x26, 0x07, 0x83, 0xe4, 0x0b, 0x16, 0x73,
		   0x16, 0x73, 0x62, 0x2a, 0xc8, 0xa5, 0xb0, 0x45,
		   0xfc, 0x3e, 0xa4, 0xaf, 0x70, 0xf7, 0x27, 0xf3,
		   0xf9, 0xe9, 0x2b, 0xdd, 0x3a, 0x1d, 0xdc, 0x42
		};
		private readonly string _privateKey1 = "9e6a6bf412ce4e3a91a33c7c0f6d94b3127b8d4f5ed336210a672fe595bf1769";
		private readonly string _privateKey2 = "b233d69dfc622ea94821f89044f5b3a82b0464a6a25177a5e39f11cfa8992009";
		private IAccountService _accountService1;
		private IAccountService _accountService2;

		[SetUp]
		public void Setup()
		{
			_accountService1 = new AccountService(_privateKey1);
			_accountService2 = new AccountService(_privateKey2);
		}

		[Test]
		public void TestEncryptAndDecrypt()
		{
			byte[] cipherData = _cryptoService.Encrypt(data, _accountService2.GetPublicKeyAsByteArray(), _accountService1.GetPrivateKeyAsByteArray());
			Assert.AreNotEqual(null, cipherData);
			Assert.AreNotEqual(data, cipherData);
			byte[] decryptedData = _cryptoService.Decrypt(cipherData, _accountService1.GetPublicKeyAsByteArray(), _accountService2.GetPrivateKeyAsByteArray());
			Assert.AreEqual(data, decryptedData);
		}

		[Test]
		public void TestGetSharedSecretValue()
		{
			ECPublicKeyParameters publicKey1 = _cryptoService.GeneratePublicKey(_accountService1.GetPublicKeyAsByteArray());
			ECPrivateKeyParameters privateKey1 = _cryptoService.GeneratePrivateKey(_accountService1.GetPrivateKeyAsByteArray());
			ECPublicKeyParameters publicKey2 = _cryptoService.GeneratePublicKey(_accountService2.GetPublicKeyAsByteArray());
			ECPrivateKeyParameters privateKey2 = _cryptoService.GeneratePrivateKey(_accountService2.GetPrivateKeyAsByteArray());
            byte[] sharedSecret1 = _cryptoService.GetSharedSecretValue(publicKey1, privateKey2);
            byte[] sharedSecret2 = _cryptoService.GetSharedSecretValue(publicKey2, privateKey1);
            Assert.AreEqual(sharedSecret1, sharedSecret2);
		}
	}
}