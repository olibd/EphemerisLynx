using System;
using System.Text;
using Lynx.Core.Services.Communications.Packets.Interfaces;
using Lynx.Core.Services.Crypto.Interfaces;

namespace Lynx.Core.Services.Crypto
{
    public class TokenCryptoService<T> : ITokenCryptoService<T> where T : IToken
    {
        private IECCCryptoService _ieccCryptoService;
        public TokenCryptoService(IECCCryptoService ieccCryptoService)
        {
            _ieccCryptoService = ieccCryptoService;
        }

        public string EncryptAndSign(T token, byte[] privkey)
        {
            throw new NotImplementedException();
        }

        public bool Verify(T token, byte[] pubkey)
        {
            byte[] unsignedEncodedToken = Encoding.UTF8.GetBytes(token.GetUnsignedEncodedToken());
            byte[] signature = Encoding.UTF8.GetBytes(token.Signature);
            return _ieccCryptoService.VerifySignedData(unsignedEncodedToken, signature, pubkey);
        }

        public void Sign(T token, byte[] privkey)
        {
            //UTF8 because the token is mostly base64 concatenated with a 
            //period so the range is ASCII which is a subset of utf-8 and 
            //Encoding.ASCII is not a class within the namespace
            byte[] encodedToken = Encoding.UTF8.GetBytes(token.GetEncodedToken());
            byte[] signature = _ieccCryptoService.GetDataSignature(encodedToken, privkey);
            token.SignAndLock(Encoding.UTF8.GetString(signature, 0, signature.Length));
        }
    }
}
