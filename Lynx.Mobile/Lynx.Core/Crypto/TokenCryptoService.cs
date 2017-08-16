using System;
using System.Collections.Generic;
using System.Text;
using Lynx.Core.Communications.Packets.Interfaces;
using Lynx.Core.Crypto.Interfaces;
using NBitcoin;
using Newtonsoft.Json;

namespace Lynx.Core.Crypto
{
    public class TokenCryptoService<T> : ITokenCryptoService<T> where T : IToken
    {
        private IECCCryptoService _ieccCryptoService;
        public TokenCryptoService(IECCCryptoService ieccCryptoService)
        {
            _ieccCryptoService = ieccCryptoService;
        }

        public string Encrypt(T token, byte[] pubkey, byte[] privkey)
        {
            byte[] encryptedPayloadBytes = _ieccCryptoService.Encrypt(Encoding.UTF8.GetBytes(token.GetEncodedPayload()), pubkey, privkey);
            string encryptedPayload = Encoding.UTF8.GetString(encryptedPayloadBytes, 0, encryptedPayloadBytes.Length);
            return token.GetEncodedHeader() + "." + encryptedPayload;
        }

        public string Decrypt(string encryptedToken, byte[] privkey)
        {
            //dissamble the encrypted token to decrypt the payload
            string[] splittedEncryptedToken = encryptedToken.Split('.');

            //get the public key
            string jsonDecodedHeader = Base64Decode(splittedEncryptedToken[0]);
            Dictionary<string, string> header = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonDecodedHeader);
            byte[] pubkey = Encoding.UTF8.GetBytes(header["pubkey"]);

            //decrypt the payload
            byte[] encryptedPayloadBytes = Encoding.UTF8.GetBytes(splittedEncryptedToken[1]);
            byte[] decryptedPayloadBytes = _ieccCryptoService.Decrypt(encryptedPayloadBytes, pubkey, privkey);
            string decryptedPayload = Encoding.UTF8.GetString(decryptedPayloadBytes, 0, decryptedPayloadBytes.Length);

            //reassemble the decrypted token
            return splittedEncryptedToken[0] + "." + decryptedPayload;
        }

        public bool Verify(T token, byte[] pubkey)
        {
            PubKey pubk = new PubKey(pubkey);
            return pubk.VerifyMessage(token.GetUnsignedEncodedToken(), token.Signature);
        }

        public void Sign(T token, byte[] privkey)
        {
            Key k = new Key(privkey);
            token.SignAndLock(k.SignMessage(token.GetEncodedToken()));
        }

        private string Base64Decode(string encodedText)
        {
            byte[] plainTextBytes = Convert.FromBase64String(encodedText);
            return System.Text.Encoding.UTF8.GetString(plainTextBytes, 0, plainTextBytes.Length);
        }
    }
}
