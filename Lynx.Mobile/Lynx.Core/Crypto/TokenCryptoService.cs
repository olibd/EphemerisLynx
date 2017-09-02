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
            byte[] encryptedPayloadBytes = _ieccCryptoService.Encrypt(Convert.FromBase64String(token.GetEncodedPayload()), pubkey, privkey);
            string encryptedPayload = Convert.ToBase64String(encryptedPayloadBytes);
            return token.GetTypedEncodedHeader() + "." + encryptedPayload;
        }

        public string Decrypt(string encryptedToken, byte[] privkey)
        {
            //dissamble the encrypted token to decrypt the payload
            string[] splittedEncryptedToken = encryptedToken.Split(':');
            string typePrefix = splittedEncryptedToken[0];
            splittedEncryptedToken = splittedEncryptedToken[1].Split('.');

            //get the public key
            string jsonDecodedHeader = Base64Decode(splittedEncryptedToken[0]);
            Dictionary<string, string> header = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonDecodedHeader);
            byte[] pubkey = Nethereum.Hex.HexConvertors.Extensions.HexByteConvertorExtensions.HexToByteArray(header["pubkey"]);

            //decrypt the payload
            byte[] encryptedPayloadBytes = Convert.FromBase64String(splittedEncryptedToken[1]);
            byte[] decryptedPayloadBytes = _ieccCryptoService.Decrypt(encryptedPayloadBytes, pubkey, privkey);
            string decryptedPayload = Convert.ToBase64String(decryptedPayloadBytes);

            //reassemble the decrypted token, add the signature if there is one
            return typePrefix + ":" + splittedEncryptedToken[0] + "." + decryptedPayload + "." + splittedEncryptedToken[2];
        }

        public bool VerifySignature(T token)
        {
            byte[] pubkey = Nethereum.Hex.HexConvertors.Extensions.HexByteConvertorExtensions.HexToByteArray(token.PublicKey);
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
