using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Lynx.Core.Communications;
using Lynx.Core.Communications.Interfaces;
using Lynx.Core.Communications.Packets;
using Lynx.Core.Communications.Packets.Interfaces;
using Lynx.Core.Crypto.Interfaces;
using Lynx.Core.Facade;
using Lynx.Core.Facade.Interfaces;
using Lynx.Core.Interfaces;
using Lynx.Core.Models.IDSubsystem;
using Lynx.Core.PeerVerification.Interfaces;
using Attribute = Lynx.Core.Models.IDSubsystem.Attribute;

namespace Lynx.Core.PeerVerification
{

    public class RequesterService : Peer, IRequesterService
    {
        private ISession _session;
        private ID _id;
        private ITokenCryptoService<IHandshakeToken> _tokenCryptoService;
        private IAccountService _accountService;
        private IIDFacade _idFacade;

        public RequesterService(ITokenCryptoService<IHandshakeToken> tokenCryptoService, IAccountService accountService, ID id, IIDFacade idFacade) : base(tokenCryptoService, accountService, idFacade)
        {
            _tokenCryptoService = tokenCryptoService;
            _accountService = accountService;
            _session = new PubNubSession(new EventHandler<string>(async (sender, e) => await ProcessEncryptedHandshakeToken<Ack>(e)));
            _id = id;
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
        private void GenerateAndSendSynAck(Ack ack)
        {
            Attribute[] accessibleAttributes = { _id.Attributes["firstname"],
                _id.Attributes["lastname"], _id.Attributes["cell"], _id.Attributes["address"]};

            SynAck synAck = new SynAck()
            {
                Id = _id,
                PublicKey = _accountService.PublicKey,
                Encrypted = true,
                AccessibleAttributes = accessibleAttributes
            };

            byte[] requesterPubKey = Encoding.UTF8.GetBytes(ack.PublicKey);
            string encryptedToken = _tokenCryptoService.Encrypt(synAck, requesterPubKey, _accountService.GetPrivateKeyAsByteArray());
            _session.Send(encryptedToken);
        }

        protected override async Task<T> ProcessEncryptedHandshakeToken<T>(string encryptedHandshakeToken)
        {
            Ack ack = await base.ProcessEncryptedHandshakeToken<Ack>(encryptedHandshakeToken);
            GenerateAndSendSynAck(ack);
            //We can return null because the caller of this method is an anonymous method in an EventHandler
            //and it won't use the returned data
            return null;
        }
    }
}
