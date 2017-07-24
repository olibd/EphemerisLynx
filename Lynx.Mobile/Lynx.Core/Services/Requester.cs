using System;
using System.Collections.Generic;
using Lynx.Core.Models.IDSubsystem;
using Lynx.Core.Services.Interfaces;
using Attribute = Lynx.Core.Models.IDSubsystem.Attribute;

namespace Lynx.Core.Services
{
    public class Requester : IRequester
    {
        private ISession _session;
        private ID id;
        private ITokenCryptoService<ISyn> _tokenCryptoService;

        public Requester(ITokenCryptoService<ISyn> tokenCryptoService)
        {
            _tokenCryptoService = tokenCryptoService;
        }

        public IAck Ack { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public string CreateEncodedSyn(IAccountService account)
        {
            ISyn syn = new Syn()
            {
                Encrypted = false,
                PublicKey = account.PublicKey(),
                NetworkAddress = _session.Open(),
                Id = id
            };

            _tokenCryptoService.Sign(syn, account.PrivateKeyAsByteArray());

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
