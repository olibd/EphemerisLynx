using System;
using System.Threading.Tasks;
using Lynx.Core.Communications.Packets;
using Lynx.Core.Communications.Packets.Interfaces;
using Lynx.Core.Crypto.Interfaces;
using Lynx.Core.Facade.Interfaces;
using Lynx.Core.Interfaces;

namespace Lynx.Core.PeerVerification
{
    public abstract class Peer
    {
        private ITokenCryptoService<IHandshakeToken> _tokenCryptoService;
        private IAccountService _accountService;
        private IIDFacade _idFacade;

        public Peer(ITokenCryptoService<IHandshakeToken> tokenCryptoService, IAccountService accountService, IIDFacade idFacade)
        {
            _accountService = accountService;
            _tokenCryptoService = tokenCryptoService;
            _idFacade = idFacade;
        }

        /// <summary>
        /// Decrypt and parses a JSON-encoded HandshakeToken
        /// </summary>
        /// <param name="encryptedHandshakeToken">The JSON-encoded SYNACK</param>
        /// <returns>The HandshakeToken object</returns>
        protected virtual async Task<T> ProcessEncryptedHandshakeToken<T>(string encryptedHandshakeToken) where T : HandshakeToken, new()
        {
            string decryptedToken = _tokenCryptoService.Decrypt(encryptedHandshakeToken, _accountService.GetPrivateKeyAsByteArray());
            HandshakeTokenFactory<T> ackTokenFactory = new HandshakeTokenFactory<T>(_idFacade);
            T handshakeToken = await ackTokenFactory.CreateHandshakeTokenAsync(decryptedToken);

            //Compare public address derived from the public key used to encrypt 
            //the token with the public address used to control the ID specify
            //in the token header. If they match then the person sending/encrypting
            //the token is the same as the one controlling the ID specified in
            //the header.
            string tokenPublicAddress = AccountService.GeneratePublicAddressFromPublicKey(handshakeToken.PublicKey);

            if (tokenPublicAddress != handshakeToken.Id.Owner)
                throw new TokenSenderIsNotIDOwnerException();

            return handshakeToken;
        }
    }
}
