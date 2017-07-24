﻿using System;
using Lynx.Core.Services.Interfaces;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Agreement;
using Org.BouncyCastle.Crypto.Agreement.Kdf;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;

namespace Lynx.Core.Services
{
    public class SECP256K1CryptoService : ISECP256K1CryptoService
    {
        private string _curveName = "secp256k1";
        private X9ECParameters _ecP;
        private ECDomainParameters _ecSpec;
        private ISigner _signer = SignerUtilities.GetSigner("SHA-256withECDSA");

        public SECP256K1CryptoService()
        {
            _ecP = NistNamedCurves.GetByName(_curveName);
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

        public byte[] GetSharedSecretValue(bool isEncrypt = true)
        {
            ECDHCBasicAgreement eLacAgreement = new ECDHCBasicAgreement();
            eLacAgreement.Init(asymmetricCipherKeyPair.Private);
            ECDHCBasicAgreement acAgreement = new ECDHCBasicAgreement();
            acAgreement.Init(asymmetricCipherKeyPairA.Private);
            BigInteger eLA = eLacAgreement.CalculateAgreement(asymmetricCipherKeyPairA.Public);
            BigInteger a = acAgreement.CalculateAgreement(asymmetricCipherKeyPair.Public);
            if (eLA.Equals(a) && !isEncrypt)
            {
                return eLA.ToByteArray();
            }
            if (eLA.Equals(a) && isEncrypt)
            {
                return a.ToByteArray();
            }
            return null;
        }

        public byte[] DeriveSymmetricKeyFromSharedSecret(byte[] sharedSecret)
        {
            ECDHKekGenerator egH = new ECDHKekGenerator(DigestUtilities.GetDigest("SHA256"));
            egH.Init(new DHKdfParameters(NistObjectIdentifiers.Aes, sharedSecret.Length, sharedSecret));
            byte[] symmetricKey = new byte[DigestUtilities.GetDigest("SHA256").GetDigestSize()];
            egH.GenerateBytes(symmetricKey, 0, symmetricKey.Length);

            return symmetricKey;
        }

        public byte[] Encrypt(byte[] data, byte[] derivedKey)
        {
            byte[] output = null;
            try
            {
                KeyParameter keyparam = ParameterUtilities.CreateKeyParameter("DES", derivedKey);
                IBufferedCipher cipher = CipherUtilities.GetCipher("DES/ECB/ISO7816_4PADDING");
                cipher.Init(true, keyparam);
                try
                {
                    output = cipher.DoFinal(data);
                    return output;
                }
                catch (System.Exception ex)
                {
                    throw new CryptoException("Invalid Data");
                }
            }
            catch (Exception ex)
            {

            }

            return output;
        }
    }
}
