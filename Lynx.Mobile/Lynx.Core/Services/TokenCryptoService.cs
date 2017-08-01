using System;
using System.Text;
using Lynx.Core.Services.Interfaces;
using NBitcoin;
using NBitcoin.DataEncoders;

namespace Lynx.Core.Services
{
    public class TokenCryptoService<T> : ITokenCryptoService<T> where T : IToken
    {

        public string EncryptAndSign(T token, byte[] privkey)
        {
            throw new NotImplementedException();
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
    }
}
