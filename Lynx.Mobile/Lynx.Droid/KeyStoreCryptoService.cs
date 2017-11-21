using System;
using System.IO;
using System.Text;
using Java.Security;
using Android.Security;
using Android.Security.Keystore;
using System.Collections.Generic;
using Java.Util;
using Java.Math;
using Javax.Security.Auth.X500;
using Javax.Crypto;

namespace Lynx.Droid
{
    public class KeyStoreCryptoService
    {
        private readonly KeyStore _keyStore;
        private readonly string _alias = "lynx";

        public KeyStoreCryptoService()
        {
            _keyStore = KeyStore.GetInstance("AndroidKeyStore");
            _keyStore.Load(null);
            if (!_keyStore.ContainsAlias(_alias))
                CreateNewKey(_alias);
        }

        private void CreateNewKey(string alias)
        {
            KeyGenParameterSpec spec = new KeyGenParameterSpec.Builder(alias, KeyStorePurpose.Decrypt | KeyStorePurpose.Encrypt)
                                                              .SetBlockModes(KeyProperties.BlockModeCbc)
                                                              .SetEncryptionPaddings(KeyProperties.EncryptionPaddingRsaPkcs1)
                                                              .Build();

            KeyPairGenerator generator = KeyPairGenerator.GetInstance("RSA", "AndroidKeyStore");
            generator.Initialize(spec);

            generator.GenerateKeyPair();
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
    }
}
