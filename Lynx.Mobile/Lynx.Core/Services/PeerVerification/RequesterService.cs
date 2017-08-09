using System;
using System.Collections.Generic;
using Lynx.Core.Models.IDSubsystem;
using Lynx.Core.Services.Communications;
using Lynx.Core.Services.Communications.Interfaces;
using Lynx.Core.Services.Communications.Packets;
using Lynx.Core.Services.Communications.Packets.Interfaces;
using Lynx.Core.Services.Crypto.Interfaces;
using Lynx.Core.Services.Interfaces;
using Lynx.Core.Services.PeerVerification.Interfaces;
using Attribute = Lynx.Core.Models.IDSubsystem.Attribute;

namespace Lynx.Core.Services.PeerVerification
{
    public class RequesterService : IRequesterService
    {
        private ISession _session;
        private ID _id;
        private ITokenCryptoService<IHandshakeToken> _tokenCryptoService;
        private IAccountService _accountService;

        public RequesterService(ITokenCryptoService<IHandshakeToken> tokenCryptoService, IAccountService accountService, ID id)
        {
            _tokenCryptoService = tokenCryptoService;
            _accountService = accountService;
            _session = new PubNubSession(new EventHandler<string>((sender, e) => ProcessEncodedAck(e)));
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

        public IAck ProcessEncodedAck(string ack)
        {
            throw new NotImplementedException();
        }

        public void SendAttributes(List<Attribute> attributes)
        {
            throw new NotImplementedException();
        }
    }
}
