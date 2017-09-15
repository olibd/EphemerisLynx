using System;
using Lynx.Core.Interfaces;
using NBitcoin.Crypto;
using Nethereum.Core.Signing.Crypto;
using System.Text;

namespace Lynx.Core
{
    public class AccountService : IAccountService
    {

        private readonly string _privateKey = "9e6a6bf412ce4e3a91a33c7c0f6d94b3127b8d4f5ed336210a672fe595bf1769";

        public AccountService()
        {

        }

        public AccountService(string privateKey)
        {
            _privateKey = privateKey;
        }

        /// <summary>
        /// Gets the etheruem account public address in LOWERCASE
        /// </summary>
        /// <returns>The account address.</returns>
        public string GetAccountAddress()
        {
            return "0x" + EthECKey.GetPublicAddress(_privateKey);
        }

        public string PrivateKey
        {
            get { return _privateKey; }
        }

        public byte[] GetPrivateKeyAsByteArray()
        {
            return Nethereum.Hex.HexConvertors.Extensions.HexByteConvertorExtensions.HexToByteArray(_privateKey);
        }

        public string PublicKey
        {
            get { return Nethereum.Hex.HexConvertors.Extensions.HexByteConvertorExtensions.ToHexCompact(GetPublicKeyAsByteArray()); }
        }

        public byte[] GetPublicKeyAsByteArray()
        {
            byte[] privK = GetPrivateKeyAsByteArray();
            ECKey eckey = new ECKey(privK, true);

            byte[] noPrefixPubKey = EthECKey.GetPubKeyNoPrefix(eckey);
            byte[] tag1 = new byte[1] { 4 };
            return CombineByteArrays(new byte[1] { 4 }, noPrefixPubKey);
        }

        public static string GeneratePublicAddressFromPublicKey(string pubK)
        {
            ECKey eckey = new ECKey(Nethereum.Hex.HexConvertors.Extensions.HexByteConvertorExtensions.HexToByteArray(pubK), false);
            string publicAddress = "0x" + EthECKey.GetPublicAddress(eckey);
            return publicAddress;
        }

        private byte[] CombineByteArrays(byte[] first, byte[] second)
        {
            byte[] ret = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, ret, 0, first.Length);
            Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);
            return ret;
        }
    }
}
