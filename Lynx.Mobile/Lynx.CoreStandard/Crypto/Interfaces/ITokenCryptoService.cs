using Lynx.Core.Communications.Packets.Interfaces;

namespace Lynx.Core.Crypto.Interfaces
{
    public interface ITokenCryptoService<T> where T : IToken
    {
        void Sign(T token, byte[] privkey);
        string Encrypt(T token, byte[] pubkey, byte[] privkey);
        bool VerifySignature(T token);
        string Decrypt(string encryptedToken, byte[] privkey);
    }
}
