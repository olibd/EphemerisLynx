using System;
using System.Text;
using Lynx.Core.Services.Interfaces;

namespace Lynx.Core.Services
{
    public class TokenCryptoService<T> : ITokenCryptoService<T> where T : IToken
    {
        private ICryptoService _secp256k1CryptoService;
        public TokenCryptoService(ICryptoService secp256k1CryptoService)
        {
            _secp256k1CryptoService = secp256k1CryptoService;
        }

        public string EncryptAndSign(T token, byte[] privkey)
        {
            throw new NotImplementedException();
        }

        public bool Verify(T token, byte[] pubkey)
        {
            byte[] unsignedEncodedToken = Encoding.UTF8.GetBytes(token.GetUnsignedEncodedToken());
            byte[] signature = Encoding.UTF8.GetBytes(token.Signature);
            return _secp256k1CryptoService.VerifySignedData(unsignedEncodedToken, signature, pubkey);
        }

        public void Sign(T token, byte[] privkey)
        {
            //UTF8 because the token is mostly base64 concatenated with a 
            //period so the range is ASCII which is a subset of utf-8 and 
            //Encoding.ASCII is not a class within the namespace
            byte[] encodedToken = Encoding.UTF8.GetBytes(token.GetEncodedToken());
            byte[] signature = _secp256k1CryptoService.GetDataSignature(encodedToken, privkey);
            token.Signature = Encoding.UTF8.GetString(signature, 0, signature.Length);
        }
    }
}
