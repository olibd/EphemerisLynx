using System;
using Lynx.Core.Services.Crypto.Interfaces;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Agreement;
using Org.BouncyCastle.Crypto.Agreement.Kdf;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Security;

namespace Lynx.Core.Services.Crypto
{
    public class SECP256K1CryptoService : IECCCryptoService
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

        private ECPublicKeyParameters GeneratePublicKey(byte[] pubkey)
        {
            FpCurve c = (FpCurve)_ecP.Curve;
            ECPoint point = _ecSpec.Curve.DecodePoint(pubkey);
            ECPublicKeyParameters publicKey = new ECPublicKeyParameters("ECDH", point, _ecSpec);
            return publicKey;
        }

        private ECPrivateKeyParameters GeneratePrivateKey(byte[] privkey)
        {
            ECPrivateKeyParameters privateKey = new ECPrivateKeyParameters("ECDH", new BigInteger(privkey), _ecSpec);
            return privateKey;
        }

        private byte[] GetSharedSecretValue(ECPublicKeyParameters publicKey, ECPrivateKeyParameters privateKey)
        {
            ECDHCBasicAgreement eLacAgreement = new ECDHCBasicAgreement();
            eLacAgreement.Init(privateKey);
            BigInteger eLA = eLacAgreement.CalculateAgreement(publicKey);
            return eLA.ToByteArray();
        }

        /// <summary>
        /// Derives the symmetric key from the shared secret.
        /// </summary>
        /// <returns>The symmetric key.</returns>
        /// <param name="sharedSecret">Shared secret.</param>
        private byte[] DeriveSymmetricKeyFromSharedSecret(byte[] sharedSecret)
        {
            ECDHKekGenerator egH = new ECDHKekGenerator(DigestUtilities.GetDigest("SHA256"));
            egH.Init(new DHKdfParameters(NistObjectIdentifiers.Aes, sharedSecret.Length, sharedSecret));
            byte[] symmetricKey = new byte[DigestUtilities.GetDigest("SHA256").GetDigestSize()];
            egH.GenerateBytes(symmetricKey, 0, symmetricKey.Length);
            return symmetricKey;
        }

        public byte[] Encrypt(byte[] data, byte[] pubkey, byte[] privkey)
        {
            byte[] output = null;
            ECPublicKeyParameters publicKey = GeneratePublicKey(pubkey);
            ECPrivateKeyParameters privateKey = GeneratePrivateKey(privkey);
            byte[] sharedSecret = GetSharedSecretValue(publicKey, privateKey);
            byte[] derivedKey = DeriveSymmetricKeyFromSharedSecret(sharedSecret);

            KeyParameter keyparam = ParameterUtilities.CreateKeyParameter("DES", derivedKey);
            IBufferedCipher cipher = CipherUtilities.GetCipher("DES/ECB/ISO7816_4PADDING");
            cipher.Init(true, keyparam);

            try
            {
                output = cipher.DoFinal(data);
                return output;
            }
            catch (InvalidCipherTextException ex)
            {
                throw new CryptoException("Invalid Data");
            }
            catch (DataLengthException ex)
            {
                throw new CryptoException("Invalid Data");
            }
        }

        public byte[] Decrypt(byte[] cipherData, byte[] pubkey, byte[] privkey)
        {
            byte[] output = null;
            ECPublicKeyParameters publicKey = GeneratePublicKey(pubkey);
            ECPrivateKeyParameters privateKey = GeneratePrivateKey(privkey);
            byte[] sharedSecret = GetSharedSecretValue(publicKey, privateKey);
            byte[] derivedKey = DeriveSymmetricKeyFromSharedSecret(sharedSecret);

            KeyParameter keyparam = ParameterUtilities.CreateKeyParameter("DES", derivedKey);
            IBufferedCipher cipher = CipherUtilities.GetCipher("DES/ECB/ISO7816_4PADDING");
            cipher.Init(false, keyparam);

            try
            {
                output = cipher.DoFinal(cipherData);
                return output;
            }
            catch (InvalidCipherTextException ex)
            {
                throw new CryptoException("Invalid Data");
            }
            catch (DataLengthException ex)
            {
                throw new CryptoException("Invalid Data");
            }
        }

        public bool VerifySignedData(byte[] data, byte[] signature, byte[] pubkey)
        {
            throw new NotImplementedException();
        }

        public byte[] GetDataSignature(byte[] data, byte[] privateKey)
        {
            throw new NotImplementedException();
        }
    }
}

