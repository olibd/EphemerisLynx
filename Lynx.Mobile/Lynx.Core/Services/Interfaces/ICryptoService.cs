using System;
namespace Lynx.Core.Services.Interfaces
{
    public interface ICryptoService
    {
        bool VerifySignedData(byte[] data, byte[] signature, byte[] pubkey);
        byte[] GetDataSignature(byte[] data, byte[] privateKey);
    }
}
