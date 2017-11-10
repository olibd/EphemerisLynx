using System;
using Lynx.Droid;
using NUnit.Framework;

namespace Lynx.Droid.UITests
{
    public class KeyStoreCryptoServiceTest
    {
        KeyStoreCryptoService keyStoreCryptoService = new KeyStoreCryptoService();

        [Test]
        public void TestEncryptAndDecryptKey()
        {
            string key = "9e6a6bf412ce4e3a91a33c7c0f6d94b3127b8d4f5ed336210a672fe595bf1769";
            string encryptedKey = keyStoreCryptoService.EncryptKey(key);
            string decryptedKey = keyStoreCryptoService.DecryptKey(encryptedKey);
            if (key != decryptedKey)
                throw new Exception("KeyStore key decryption failed");
        }

        [STAThread]
        static void Main(params string[] paramaters)
        { }
    }
}
