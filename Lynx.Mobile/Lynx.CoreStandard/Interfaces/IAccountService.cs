namespace Lynx.Core.Interfaces
{
    public interface IAccountService
    {
        string PrivateKey { get; }
        string PublicKey { get; }
        string MnemonicPhrase { get; }
        byte[] GetPrivateKeyAsByteArray();
        byte[] GetPublicKeyAsByteArray();
        string GetAccountAddress();
    }
}
