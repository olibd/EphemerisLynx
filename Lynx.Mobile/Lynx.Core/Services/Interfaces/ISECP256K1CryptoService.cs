using System;
namespace Lynx.Core.Services.Interfaces
{
    public interface ISECP256K1CryptoService
    {
        bool VerifySignedData(byte[] data, byte[] signature, byte[] pubkey);
        byte[] GetDataSignature(byte[] data, byte[] privateKey);
    }
}
