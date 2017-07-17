using System;
namespace Lynx.Core.Services.Interfaces
{
    public interface IAccountService
    {
        string PrivateKey();
        string PublicKey();
        byte[] PrivateKeyAsByteArray();
        byte[] PublicKeyAsByteArray();
        string accountAddress();
    }
}
