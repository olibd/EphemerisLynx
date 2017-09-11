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
    public class Verifier : Peer, IVerifier
    {
        private ITokenCryptoService<IToken> _tokenCryptoService;
        private ID _id;
        private IAccountService _accountService;
        private ISession _session;
        private IIDFacade _idFacade;
        private ISynAck _synAck;
        private ICertificateFacade _certificateFacade;
        public ISynAck SynAck { get { return _synAck; } }

        public event EventHandler<IdentityProfileReceivedEvent> IdentityProfileReceived;

        public Verifier(ITokenCryptoService<IToken> tokenCryptoService, IAccountService accountService, ID id, IIDFacade idFacade, ICertificateFacade certificateFacade) : base(tokenCryptoService, accountService, idFacade)
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
                Acknowledge(syn);
            else
                throw new SignatureDoesntMatchException("The signature was not " +
                                                        "generated by the given " +
                                                        "public Key");
        }

        protected override async Task<T> ProcessEncryptedHandshakeToken<T>(string encryptedHandshakeToken)
        {
            SynAck unverifiedSynAck = await base.ProcessEncryptedHandshakeToken<SynAck>(encryptedHandshakeToken);

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
            //We can return null because the caller of this method is an anonymous method in an EventHandler
            //and it won't use the returned data
            return null;
        }

        protected override async Task<T> ProcessEncryptedInfoRequestToken<T>(string encryptedInfoRequestToken)
		{
            InfoRequestSynAck infoRequestSynAck = await base.ProcessEncryptedInfoRequestToken<InfoRequestSynAck>(encryptedInfoRequestToken);

            if (_tokenCryptoService.VerifySignature(infoRequestSynAck))
                GenerateAndSendResponse(infoRequestSynAck);
            else
				throw new SignatureDoesntMatchException("The signature was not " +
																		"generated by the given " +
																		"public Key");
            return null; 
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

            SynAck response = new SynAck()
            {
                Id = _id,
                PublicKey = _accountService.PublicKey,
                Encrypted = true,
                AccessibleAttributes = requestedAttributes
			};

			byte[] requesterPubKey = Nethereum.Hex.HexConvertors.Extensions.HexByteConvertorExtensions.HexToByteArray(infoRequestSynAck.PublicKey);
			string encryptedToken = _tokenCryptoService.Encrypt(response, requesterPubKey, _accountService.GetPrivateKeyAsByteArray());
			_session.Send(encryptedToken);
		}


		public async Task Certify(string[] keysOfAttributesToCertifify)
        {
            Certificate[] certificates = await IssueCertificates(keysOfAttributesToCertifify);

            string encryptedToken = "cert:" + CreateEncryptedCertificationConfirmationToken(certificates);

            _session.Send(encryptedToken);
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
                };

                await _certificateFacade.DeployAsync(cert);

                certificates[i] = cert;
                i++;
            }

            return certificates;
        }
    }
}
