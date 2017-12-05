using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Lynx.Core.Communications;
using Lynx.Core.Communications.Interfaces;
using Lynx.Core.Communications.Packets;
using Lynx.Core.Communications.Packets.Interfaces;
using Lynx.Core.Crypto;
using Lynx.Core.Crypto.Interfaces;
using Lynx.Core.Facade;
using Lynx.Core.Facade.Interfaces;
using Lynx.Core.Interfaces;
using Lynx.Core.Models.IDSubsystem;
using Lynx.Core.PeerVerification.Interfaces;
using Attribute = Lynx.Core.Models.IDSubsystem.Attribute;
using eVi.abi.lib.pcl;
namespace Lynx.Core.PeerVerification
{

    public class Requester : Peer, IRequester
    {
        protected ISession _session;
        private ID _id;
        protected ITokenCryptoService<IToken> _tokenCryptoService;
        protected IAccountService _accountService;
        private ICertificateFacade _certificateFacade;
        protected Attribute[] _accessibleAttributes;
        private IAttributeFacade _attributeFacade;

        public event EventHandler<IssuedCertificatesAddedToIDEvent> HandshakeComplete;

        public Requester(ITokenCryptoService<IToken> tokenCryptoService, IAccountService accountService, ID id, IIDFacade idFacade, IAttributeFacade attributeFacade, ICertificateFacade certificateFacade) : base(tokenCryptoService, accountService, idFacade)
        {
            _tokenCryptoService = tokenCryptoService;
            _accountService = accountService;
            _session = new AblySession(new EventHandler<string>(async (sender, e) => await RouteEncryptedHandshakeToken<Ack>(e)), id.Address);
            _id = id;
            _attributeFacade = attributeFacade;
            _certificateFacade = certificateFacade;
            _accessibleAttributes = new Attribute[_id.Attributes.Values.Count];
            _id.Attributes.Values.CopyTo(_accessibleAttributes, 0);
        }

        public IAck Ack { get; set; }

        public string CreateEncodedSyn()
        {
            ISyn syn = new Syn()
            {
                Encrypted = false,
                PublicKey = _accountService.PublicKey,
                NetworkAddress = _session.Open(),
                Id = _id
            };

            _tokenCryptoService.Sign(syn, _accountService.GetPrivateKeyAsByteArray());

            return syn.GetEncodedToken();
        }

        /// <summary>
        /// JSON-Encodes and sends attributes and attribute contents to the verifier for certification
        /// </summary>
        protected virtual void GenerateAndSendSynAck(Ack ack)
        {
            SynAck synAck = new SynAck()
            {
                Id = _id,
                PublicKey = _accountService.PublicKey,
                Encrypted = true,
                AccessibleAttributes = _accessibleAttributes
            };

            byte[] requesterPubKey = Nethereum.Hex.HexConvertors.Extensions.HexByteConvertorExtensions.HexToByteArray(ack.PublicKey);
            _tokenCryptoService.Sign(synAck, _accountService.GetPrivateKeyAsByteArray());
            string encryptedToken = _tokenCryptoService.Encrypt(synAck, requesterPubKey, _accountService.GetPrivateKeyAsByteArray());
            _session.Send(encryptedToken);
        }

        protected virtual async Task RouteEncryptedHandshakeToken<T>(string encryptedHandshakeToken)
        {
            try
            {
                string[] tokenArr = encryptedHandshakeToken.Split(':');

                switch (tokenArr[0])
                {
                    case "ack":
                        await ProcessAck(encryptedHandshakeToken);
                        break;

                    case "cert":
                        await ProcessCertificationConfirmationToken(encryptedHandshakeToken);
                        break;

                    default:
                        throw new InvalidTokenTypeException("The other peer sent invalid data");
                }
            }
            catch (UserFacingException e)
            {
                RaiseError(e);
            }
        }

        protected async Task ProcessAck(string encryptedToken)
        {
            Ack ack = await base.DecryptAndInstantiateHandshakeToken<Ack>(encryptedToken);

            VerifyHandshakeTokenIDOwnership(ack);

            if (_tokenCryptoService.VerifySignature(ack))
            {
                Ack = ack;
                GenerateAndSendSynAck(ack);
            }
            else
                throw new SignatureDoesntMatchException("Unable to validate the other peer's signature");
        }

        private async Task ProcessCertificationConfirmationToken(string encryptedToken)
        {
            string decryptedToken = _tokenCryptoService.Decrypt(encryptedToken, _accountService.GetPrivateKeyAsByteArray());
            CertificationConfirmationTokenFactory tokenFactory = new CertificationConfirmationTokenFactory(_certificateFacade);

            CertificationConfirmationToken token = null;
            try
            {
                token = await tokenFactory.CreateTokenAsync(decryptedToken);
            }
            catch (CallFailed e)
            {
                throw new FailedBlockchainDataAcess("Unable to recover the certificate(s) data.", e);
            }

            if (token.PublicKey != Ack.PublicKey)
                throw new TokenPublicKeyMismatch();

            if (_tokenCryptoService.VerifySignature(token))
                await AddCertificatesToTheAccessibleAttributes(token.IssuedCertificates);
            else
                throw new SignatureDoesntMatchException("Unable to validate the other peer's signature");
        }

        private async Task AddCertificatesToTheAccessibleAttributes(Certificate[] certificates)
        {
            _session.Close();

            List<Certificate> addedCertificate = new List<Certificate>();
            List<Certificate> unaddedCertificate = new List<Certificate>();

            foreach (Attribute attr in _accessibleAttributes)
            {
                foreach (Certificate cert in certificates)
                {
                    if (attr.Address != cert.OwningAttribute.Address)
                        continue;

                    cert.OwningAttribute = attr;

                    //overwrite the old certificate
                    if (attr.Certificates.ContainsKey(cert.Owner))
                        attr.Certificates.Remove(cert.Owner);

                    attr.AddCertificate(cert);

                    try
                    {
                        await _attributeFacade.AddCertificateAsync(attr, cert);
                    }
                    catch (TransactionFailed ex)
                    {
                        unaddedCertificate.Add(cert);
                        continue;
                    }

                    addedCertificate.Add(cert);
                }
            }

            IssuedCertificatesAddedToIDEvent ev = new IssuedCertificatesAddedToIDEvent()
            {
                CertificatesAdded = addedCertificate
            };

            if (unaddedCertificate.Count > 0)
            {
                PartialAdditionOfCertificatesException exception = new PartialAdditionOfCertificatesException(unaddedCertificate.ToArray())
                {
                    SuccessfulTransactions = addedCertificate.ToArray()
                };

                RaiseError(exception);
            }
            else
                HandshakeComplete.Invoke(this, ev);
        }

        public void ResumeSession(string sessionID)
        {
            _session.Open(sessionID);
        }

        public string SuspendSession()
        {
            string sId = _session.SessionID;
            _session.Close();
            return sId;
        }
    }
}
