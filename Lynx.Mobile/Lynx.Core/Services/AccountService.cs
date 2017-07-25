using System;
using Lynx.Core.Services.Interfaces;
using Nethereum.Core.Signing.Crypto;

namespace Lynx.Core.Services
{
    public class AccountService : IAccountService
    {

        private string _privateKey = "9e6a6bf412ce4e3a91a33c7c0f6d94b3127b8d4f5ed336210a672fe595bf1769";

        public AccountService()
        {

        }

        public AccountService(string privKey)
        {
            _privateKey = privKey;
        }

        public string AccountAddress()
        {
            return EthECKey.GetPublicAddress(_privateKey);
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
            var eckey = new NBitcoin.Crypto.ECKey(privK, true);
            return EthECKey.GetPubKeyNoPrefix(eckey);
        }
    }
}
