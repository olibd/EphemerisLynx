namespace Lynx.Core.Services.Crypto.Interfaces
{
    public interface IECCCryptoService
    {
        bool VerifySignedData(byte[] data, byte[] signature, byte[] pubkey);
        byte[] GetDataSignature(byte[] data, byte[] privateKey);

        /// <summary>
        /// Encrypt the specified data, pubkey and privkey.
        /// </summary>
        /// <returns>The encrypted ciphertext.</returns>
        /// <param name="data">Data.</param>
        /// <param name="pubkey">Public key.</param>
        /// <param name="privkey">Private key.</param>
        byte[] Encrypt(byte[] data, byte[] pubkey, byte[] privkey);

        /// <summary>
        /// Decrypt the specified cipherData, pubkey and privkey.
        /// </summary>
        /// <returns>The decrypted data</returns>
        /// <param name="cipherData">Cipher data.</param>
        /// <param name="pubkey">Public key.</param>
        /// <param name="privkey">Private key.</param>
        byte[] Decrypt(byte[] cipherData, byte[] pubkey, byte[] privkey);
    }
}
