using System;
using System.Text;
using Lynx.Core.Communications.Interfaces;
using Lynx.Core.Communications.Packets;
using Lynx.Core.Communications.Packets.Interfaces;
using Lynx.Core.Crypto;
using Lynx.Core.Crypto.Interfaces;
using Lynx.Core.Interfaces;
using Lynx.Core.Models.IDSubsystem;
using Lynx.Core.PeerVerification.Interfaces;
using Attribute = Lynx.Core.Models.IDSubsystem.Attribute;
using Lynx.Core.Facade;
using System.Threading.Tasks;
using Lynx.Core.Communications;
using Lynx.Core.Facade.Interfaces;

namespace Lynx.Core.PeerVerification
{
    public class Receiver : Peer, IReceiver
    {
        private ITokenCryptoService<IToken> _tokenCryptoService;
        private ID _id;
        private IAccountService _accountService;
        private ISession _session;
        private IIDFacade _idFacade;
        private ISynAck _synAck;
        private ISyn _syn;
        private ICertificateFacade _certificateFacade;
        public ISynAck SynAck { get { return _synAck; } }

        public event EventHandler<IdentityProfileReceivedEvent> IdentityProfileReceived;
        public event EventHandler<CertificatesSent> CertificatesSent;

        public Receiver(ITokenCryptoService<IToken> tokenCryptoService, IAccountService accountService, ID id, IIDFacade idFacade, ICertificateFacade certificateFacade) : base(tokenCryptoService, accountService, idFacade)
        {
            _tokenCryptoService = tokenCryptoService;
            _id = id;
            _accountService = accountService;
            _idFacade = idFacade;
            _certificateFacade = certificateFacade;
            _session = new PubNubSession(new EventHandler<string>(async (sender, e) => await ProcessEncryptedHandshakeToken<SynAck>(e)));
        }

        /// <summary>
        /// Creates and transmits an ACK in response to a previously processed SYN
        /// </summary>
        private void Acknowledge(ISyn syn)
        {
            Attribute[] accessibleAttributes = { _id.Attributes["firstname"], _id.Attributes["lastname"] };

            Ack ack = new Ack()
            {
                Id = _id,
                PublicKey = _accountService.PublicKey,
                Encrypted = true,
                AccessibleAttributes = accessibleAttributes
            };

            byte[] requesterPubKey = Nethereum.Hex.HexConvertors.Extensions.HexByteConvertorExtensions.HexToByteArray(syn.PublicKey);
            _tokenCryptoService.Sign(ack, _accountService.GetPrivateKeyAsByteArray());
            string encryptedToken = _tokenCryptoService.Encrypt(ack, requesterPubKey, _accountService.GetPrivateKeyAsByteArray());
            _session.Open(syn.NetworkAddress);
            _session.Send(encryptedToken);
        }

        public async Task ProcessSyn(string synString)
        {
            HandshakeTokenFactory<Syn> synFactory = new HandshakeTokenFactory<Syn>(_idFacade);
            Syn syn = await synFactory.CreateHandshakeTokenAsync(synString);

            byte[] pubK = Nethereum.Hex.HexConvertors.Extensions.HexByteConvertorExtensions.HexToByteArray(syn.PublicKey);

            VerifyHandshakeTokenIDOwnership(syn);

            if (_tokenCryptoService.VerifySignature(syn))
            {
                _syn = syn;
                Acknowledge(_syn);
            }
            else
                throw new SignatureDoesntMatchException("The signature was not " +
                                                        "generated by the given " +
                                                        "public Key");
        }

        protected override async Task<T> ProcessEncryptedHandshakeToken<T>(string encryptedHandshakeToken)
        {
            string[] tokenArr = encryptedHandshakeToken.Split(':');

            switch (tokenArr[0])
            {
                case "synack":
                    await ProcessSynAck(encryptedHandshakeToken);
                    break;

                case "inforeqsynack":
                    await ProcessInfoRequestSynAck(encryptedHandshakeToken);
                    break;

                default:
                    throw new InvalidTokenTypeException("The Token type received is invalid");
            }

            //We can return null because the caller of this method is an anonymous method in an EventHandler
            //and it won't use the returned data
            return null;
        }

        private async Task ProcessSynAck(string encryptedSynAck)
        {
            SynAck unverifiedSynAck = await base.ProcessEncryptedHandshakeToken<SynAck>(encryptedSynAck);


            if (unverifiedSynAck.PublicKey != _syn.PublicKey)
                throw new TokenPublicKeyMismatch();

            if (!_tokenCryptoService.VerifySignature(unverifiedSynAck))
                throw new SignatureDoesntMatchException("The signature was not " +
                                                                        "generated by the given " +
                                                                        "public Key");

            _synAck = unverifiedSynAck;

            IdentityProfileReceivedEvent e = new IdentityProfileReceivedEvent()
            {
                SynAck = _synAck
            };
            IdentityProfileReceived.Invoke(this, e);
        }

        private async Task ProcessInfoRequestSynAck(string encryptedInfoRequestToken)
        {

            string decryptedToken = _tokenCryptoService.Decrypt(encryptedInfoRequestToken, _accountService.GetPrivateKeyAsByteArray());
            InfoRequestTokenFactory<InfoRequestSynAck> handshakeTokenFactory = new InfoRequestTokenFactory<InfoRequestSynAck>(_idFacade);
            InfoRequestSynAck infoRequestSynAck = await handshakeTokenFactory.CreateInfoRequestTokenAsync(decryptedToken);

            if (infoRequestSynAck.PublicKey != _syn.PublicKey)
                throw new TokenPublicKeyMismatch();

            if (_tokenCryptoService.VerifySignature(infoRequestSynAck))
                GenerateAndSendResponse(infoRequestSynAck);
            else
                throw new SignatureDoesntMatchException("The signature was not " +
                                                                        "generated by the given " +
                                                                        "public Key");
        }

        /// <summary>
        /// JSON-Encodes and sends attributes and attribute contents to the requesting service
        /// </summary>
        private void GenerateAndSendResponse(InfoRequestSynAck infoRequestSynAck)
        {
            string[] requestedAttributesDescription = infoRequestSynAck.RequestedAttributes;
            Attribute[] requestedAttributes = new Attribute[requestedAttributesDescription.Length];
            for (int i = 0; i < requestedAttributesDescription.Length; i++)
            {
                requestedAttributes[i] = _id.Attributes[requestedAttributesDescription[i]];
            }

            InfoRequestResponse response = new InfoRequestResponse()
            {
                Id = _id,
                PublicKey = _accountService.PublicKey,
                Encrypted = true,
                AccessibleAttributes = requestedAttributes
            };

            byte[] requesterPubKey = Nethereum.Hex.HexConvertors.Extensions.HexByteConvertorExtensions.HexToByteArray(infoRequestSynAck.PublicKey);
            _tokenCryptoService.Sign(response, _accountService.GetPrivateKeyAsByteArray());
            string encryptedToken = _tokenCryptoService.Encrypt(response, requesterPubKey, _accountService.GetPrivateKeyAsByteArray());
            _session.Send(encryptedToken);
        }


        public async Task Certify(string[] keysOfAttributesToCertifify)
        {
            Certificate[] certificates = await IssueCertificates(keysOfAttributesToCertifify);

            string encryptedToken = CreateEncryptedCertificationConfirmationToken(certificates);

            _session.Send(encryptedToken);

            CertificatesSent.Invoke(this, new CertificatesSent());
        }

        private string CreateEncryptedCertificationConfirmationToken(Certificate[] certificates)
        {
            CertificationConfirmationToken certConfToken = new CertificationConfirmationToken()
            {
                PublicKey = _accountService.PublicKey,
                Encrypted = true,
                IssuedCertificates = certificates
            };

            byte[] requesterPubKey = Nethereum.Hex.HexConvertors.Extensions.HexByteConvertorExtensions.HexToByteArray(SynAck.PublicKey);
            _tokenCryptoService.Sign(certConfToken, _accountService.GetPrivateKeyAsByteArray());
            return _tokenCryptoService.Encrypt(certConfToken, requesterPubKey, _accountService.GetPrivateKeyAsByteArray());
        }

        private async Task<Certificate[]> IssueCertificates(string[] attributeKeys)
        {
            Certificate[] certificates = new Certificate[attributeKeys.Length];

            int i = 0;
            foreach (string key in attributeKeys)
            {
                Attribute attr = SynAck.Id.Attributes[key];
                Certificate cert = new Certificate()
                {
                    OwningAttribute = attr,
                    Revoked = false,
                    Location = "CertFor" + attr.Description,
                    Hash = "HashFor" + attr.Description
                };

                await _certificateFacade.DeployAsync(cert);

                certificates[i] = cert;
                i++;
            }

            return certificates;
        }
    }
}