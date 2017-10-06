using System;
using Lynx.Core.Interfaces;
using NBitcoin.Crypto;
using Nethereum.Core.Signing.Crypto;
using System.Text;
using Lynx.Core.Crypto;
using NBitcoin;
using NBitcoin.Protocol;
using Org.BouncyCastle.Security;

namespace Lynx.Core
{
    public class AccountService : IAccountService
    {

        private readonly string _privateKey;
        private Mnemonic _mnemonic;

        public string MnemonicPhrase
        {
            get => _mnemonic.ToString();
        }

        public AccountService()
        {
            _mnemonic = new Mnemonic(Wordlist.English, WordCount.Twelve);

            Key privKey = _mnemonic.DeriveExtKey().PrivateKey;
            _privateKey = Nethereum.Hex.HexConvertors.Extensions.HexByteConvertorExtensions.ToHex(privKey.ToBytes(ProtocolVersion
                .BIP0031_VERSION));
        }

        public AccountService(Mnemonic mnemonic)
        {
            _mnemonic = mnemonic;
            Key privKey = mnemonic.DeriveExtKey().PrivateKey;
            _privateKey = Nethereum.Hex.HexConvertors.Extensions.HexByteConvertorExtensions.ToHex(privKey.ToBytes(ProtocolVersion
                .BIP0031_VERSION));
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
