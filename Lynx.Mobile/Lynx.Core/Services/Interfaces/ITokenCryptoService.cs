using System;
namespace Lynx.Core.Services.Interfaces
{
    public interface ITokenCryptoService<T> where T : IToken
    {
        T Sign(T token);
        String EncryptAndSign(T token);
    }
}
