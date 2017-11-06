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
        private String encryptedText;
        private Locale local = Locale.Canada;
        private byte[] vals;
        //private List<Object> aliaslist;

        public KeyStoreCryptoService()
        {
            _keyStore = KeyStore.GetInstance("AndroidKeyStore");
            _keyStore.Load(null);
            //aliaslist = new List<Object>();
            //var alias = _keyStore.Aliases();
            CreateNewKey();
            //while (alias.HasMoreElements)
            //{
            //    aliaslist.Add(alias.NextElement());
            //}
        }

        private void CreateNewKey()
        {
            if (!_keyStore.ContainsAlias(_alias))
            {
                Calendar start = Calendar.GetInstance(local);
                Calendar end = Calendar.GetInstance(local);
                end.Add(CalendarField.Year, 1);
                KeyPairGeneratorSpec spec = new KeyPairGeneratorSpec.Builder(new Android.App.Application())
                            .SetAlias(_alias)
                            .SetSubject(new X500Principal("CN=lynx, O=Android Authority"))
                            .SetSerialNumber(BigInteger.One)
                            .SetStartDate(start.Time)
                            .SetEndDate(end.Time)
                            .Build();
                KeyPairGenerator generator = KeyPairGenerator.GetInstance("RSA", "AndroidKeyStore");
                generator.Initialize(spec);

                //KeyPair keyPair = generator.GenerateKeyPair();
            }
        }

        public void EncryptAndSaveKey(string privKey)
        {
            KeyStore.PrivateKeyEntry privateKeyEntry = (KeyStore.PrivateKeyEntry) _keyStore.GetEntry(_alias, null);
            var publicKey = privateKeyEntry.Certificate.PublicKey;//AES/CBC/PKCS7PADDING

            Cipher cipher = Cipher.GetInstance("RSA/ECB/PKCS1Padding", "AndroidOpenSSL");
            cipher.Init(CipherMode.EncryptMode, publicKey);//

            MemoryStream outputStream = new MemoryStream();
            CipherOutputStream cipherOutputStream = new CipherOutputStream(
                outputStream, cipher);
            cipherOutputStream.Write(Encoding.UTF8.GetBytes(privKey));
            cipherOutputStream.Close();

            vals = outputStream.ToArray();
        }

        public string DecryptAndGetKey()
        {
            KeyStore.PrivateKeyEntry privateKeyEntry = (KeyStore.PrivateKeyEntry)_keyStore.GetEntry(_alias, null);
            var privateKey = privateKeyEntry.PrivateKey;//RSA/ECB/PKCS1Padding

            Cipher cipher = Cipher.GetInstance("RSA/ECB/PKCS1Padding", "AndroidOpenSSL");
            cipher.Init(CipherMode.DecryptMode, privateKey);

            CipherInputStream cipherInputStream = new CipherInputStream(
                new MemoryStream(vals), cipher);

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
