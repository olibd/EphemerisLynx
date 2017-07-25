using System;
using Lynx.Core.Services.Interfaces;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Agreement;
using Org.BouncyCastle.Crypto.Agreement.Kdf;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;

namespace Lynx.Core.Services
{
    public class SECP256K1CryptoService : ICryptoService
    {
        private string _curveName = "secp256k1";
        private X9ECParameters _ecP;
        private ECDomainParameters _ecSpec;
        private ISigner _signer = SignerUtilities.GetSigner("SHA-256withECDSA");

        public SECP256K1CryptoService()
        {
            _ecP = SecNamedCurves.GetByName(_curveName);
            _ecSpec = new ECDomainParameters(_ecP.Curve, _ecP.G, _ecP.N, _ecP.H, _ecP.GetSeed());
        }

        public byte[] GetDataSignature(byte[] data, byte[] privateKey)
        {
            //Adapted From: https://stackoverflow.com/questions/17439732/recreating-keys-ecpublickeyparameters-in-c-sharp-with-bouncycastle
            BigInteger biPrivateKey = new BigInteger(privateKey);
            ECPrivateKeyParameters keyParameters = new ECPrivateKeyParameters(biPrivateKey, _ecSpec);
            _signer.Init(true, keyParameters);
            _signer.BlockUpdate(data, 0, data.Length);
            return _signer.GenerateSignature();
        }

        public bool VerifySignedData(byte[] data, byte[] signature, byte[] pubkey)
        {
            ECPublicKeyParameters ecpubKey = (ECPublicKeyParameters)PublicKeyFactory.CreateKey(pubkey);
            _signer.Init(false, ecpubKey);
            _signer.BlockUpdate(data, 0, data.Length);
            return _signer.VerifySignature(signature);
        }

    }
}
