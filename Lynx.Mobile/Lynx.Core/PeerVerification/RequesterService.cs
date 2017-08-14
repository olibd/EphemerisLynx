using System;
using System.Collections.Generic;
using Lynx.Core.Communications;
using Lynx.Core.Communications.Interfaces;
using Lynx.Core.Communications.Packets;
using Lynx.Core.Communications.Packets.Interfaces;
using Lynx.Core.Crypto.Interfaces;
using Lynx.Core.Facade;
using Lynx.Core.Interfaces;
using Lynx.Core.Models.IDSubsystem;
using Lynx.Core.PeerVerification.Interfaces;
using Attribute = Lynx.Core.Models.IDSubsystem.Attribute;

namespace Lynx.Core.PeerVerification
{

    public class RequesterService : IRequesterService
    {
        private ISession _session;
        private ID _id;
        private ITokenCryptoService<IHandshakeToken> _tokenCryptoService;
        private IAccountService _accountService;
        private IDFacade _idFacade;

        public RequesterService(ITokenCryptoService<IHandshakeToken> tokenCryptoService, IAccountService accountService, ID id, IDFacade idFacade)
        {
            _tokenCryptoService = tokenCryptoService;
            _accountService = accountService;
            _session = new PubNubSession(new EventHandler<string>((sender, e) => ProcessEncryptedAck(e)));
            _id = id;
            _idFacade = idFacade;
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

        public IAck ProcessEncryptedAck(string ack)
        {
            string decryptedToken = _tokenCryptoService.Decrypt(ack, _accountService.GetPrivateKeyAsByteArray());
            Ack ackObj = new Ack(decryptedToken);
            //TODO: check pub key agaisnt ID
            ackObj
        }

        public void SendAttributes(List<Attribute> attributes)
        {
            throw new NotImplementedException();
        }
    }
}
