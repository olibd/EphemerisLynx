using System;
using System.IO;
using System.Text;
using Java.Security;
using Android.Security;
using System.Collections.Generic;
using Java.Util;
using Java.Math;
using Javax.Security.Auth.X500;
using Javax.Crypto;

namespace Lynx.Droid
{
    public class KeyStoreCryptoService
    {
        readonly KeyStore _keyStore;
        private readonly string _alias = "lynx";
        private Locale local = Locale.Canada;

        public KeyStoreCryptoService()
        {
            _keyStore = KeyStore.GetInstance("AndroidKeyStore");
            _keyStore.Load(null);
            CreateNewKey();
        }

        private void CreateNewKey()
        {
            if (!_keyStore.ContainsAlias(_alias))
            {
                Calendar start = Calendar.GetInstance(local);
                Calendar end = Calendar.GetInstance(local);
                end.Add(CalendarField.Year, 30);

                KeyPairGeneratorSpec spec = new KeyPairGeneratorSpec.Builder(new Android.App.Application())
                            .SetAlias(_alias)
                            .SetSubject(new X500Principal("CN=lynx, O=Android Authority"))
                            .SetSerialNumber(BigInteger.One)
                            .SetStartDate(start.Time)
                            .SetEndDate(end.Time)
                            .Build();
                KeyPairGenerator generator = KeyPairGenerator.GetInstance("RSA", "AndroidKeyStore");
                generator.Initialize(spec);

                generator.GenerateKeyPair();
            }
        }

        public string EncryptKey(string privKey)
        {
            KeyStore.PrivateKeyEntry privateKeyEntry = (KeyStore.PrivateKeyEntry) _keyStore.GetEntry(_alias, null);
            var publicKey = privateKeyEntry.Certificate.PublicKey;

            Cipher cipher = Cipher.GetInstance("RSA/ECB/PKCS1Padding", "AndroidOpenSSL");
            cipher.Init(CipherMode.EncryptMode, publicKey);//

            MemoryStream outputStream = new MemoryStream();
            CipherOutputStream cipherOutputStream = new CipherOutputStream(
                outputStream, cipher);
            cipherOutputStream.Write(Encoding.UTF8.GetBytes(privKey));
            cipherOutputStream.Close();

            byte[] encryptedBytes = outputStream.ToArray();
            return Nethereum.Hex.HexConvertors.Extensions.HexByteConvertorExtensions.ToHexCompact(encryptedBytes);
        }

        public string DecryptKey(string encryptedKey)
        {
            KeyStore.PrivateKeyEntry privateKeyEntry = (KeyStore.PrivateKeyEntry)_keyStore.GetEntry(_alias, null);

            if (privateKeyEntry == null)
                return null;
            
            var privateKey = privateKeyEntry.PrivateKey;
            Cipher cipher = Cipher.GetInstance("RSA/ECB/PKCS1Padding");
            cipher.Init(CipherMode.DecryptMode, privateKey);

            byte[] encryptedBytes = Nethereum.Hex.HexConvertors.Extensions.HexByteConvertorExtensions.HexToByteArray(encryptedKey);

            CipherInputStream cipherInputStream = new CipherInputStream(
                new MemoryStream(encryptedBytes), cipher);

            List<byte> values = new List<byte>();

            int nextByte;
            while ((nextByte = cipherInputStream.Read()) != -1)
            {
                values.Add((byte)nextByte);
            }

            return Encoding.UTF8.GetString(values.ToArray(), 0, values.Count);
        }

        public void RemoveKey()
        {
            _keyStore.DeleteEntry(_alias);
        }

        public void TestEncryptAndDecryptKey()
        {
            string key = "9e6a6bf412ce4e3a91a33c7c0f6d94b3127b8d4f5ed336210a672fe595bf1769";
            string encryptedKey = EncryptKey(key);
            string decryptedKey = DecryptKey(encryptedKey);
            if (key != decryptedKey)
                throw new Exception("KeyStore key decryption failed");
        }
    }
}
