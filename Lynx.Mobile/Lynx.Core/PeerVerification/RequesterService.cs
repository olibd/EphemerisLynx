using System;
using System.Collections.Generic;
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

    public class RequesterService : IRequesterService
    {
        private ISession _session;
        private ID _id;
        private ITokenCryptoService<IHandshakeToken> _tokenCryptoService;
        private IAccountService _accountService;
        private IIDFacade _idFacade;

        public RequesterService(ITokenCryptoService<IHandshakeToken> tokenCryptoService, IAccountService accountService, ID id, IIDFacade idFacade)
        {
            _tokenCryptoService = tokenCryptoService;
            _accountService = accountService;
            _session = new PubNubSession(new EventHandler<string>(async (sender, e) => await ProcessEncryptedAckAsync(e)));
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

        public async Task ProcessEncryptedAckAsync(string ack)
        {
            string decryptedToken = _tokenCryptoService.Decrypt(ack, _accountService.GetPrivateKeyAsByteArray());
            HandshakeTokenFactory<Ack> ackTokenFactory = new HandshakeTokenFactory<Ack>(_idFacade);
            Ack ackObj = await ackTokenFactory.CreateHandshakeTokenAsync(decryptedToken);

            //Compare public address derived from the public key used to encrypt 
            //the token with the public address used to control the ID specify
            //in the token header. If they match then the person sending/encrypting
            //the token is the same as the one controlling the ID specified in
            //the header.
            string tokenPublicAddress = AccountService.GeneratePublicAddressFromPublicKey(ackObj.PublicKey);

            if (tokenPublicAddress != ackObj.Id.Owner)
                throw new TokenSenderIsNotIDOwnerException();

            //TODO: call display of the name
            //TODO: send SYNACK
        }

        public void SendAttributes(List<Attribute> attributes)
        {
            throw new NotImplementedException();
        }
    }
}
