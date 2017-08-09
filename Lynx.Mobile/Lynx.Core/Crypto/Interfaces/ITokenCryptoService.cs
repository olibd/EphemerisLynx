using Lynx.Core.Communications.Packets.Interfaces;

namespace Lynx.Core.Crypto.Interfaces
{
    public interface ITokenCryptoService<T> where T : IToken
    {
        void Sign(T token, byte[] privkey);
        string EncryptAndSign(T token, byte[] privkey);
        bool Verify(T token, byte[] pubkey);
    }
}
