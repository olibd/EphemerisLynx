using System;
namespace Lynx.Core.Crypto.Interfaces
{
    public interface IKeyStoreCryptoService
    {
        void EncryptAndSaveKey(string privKey);
        string DecryptAndGetKey();

    }
}
