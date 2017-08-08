using System;
namespace Lynx.Core.Services.Interfaces
{
    public interface IAccountService
    {
        string PrivateKey { get; }
        string PublicKey { get; }
        byte[] GetPrivateKeyAsByteArray();
        byte[] GetPublicKeyAsByteArray();
        string GetAccountAddress();
    }
}
