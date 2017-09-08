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
        private ITokenCryptoService<IHandshakeToken> _handshakeTokenCryptoService;
        private ITokenCryptoService<IInfoRequestToken> _infoRequestTokenCryptoService;
        private IAccountService _accountService;
        private IIDFacade _idFacade;

        public Peer(ITokenCryptoService<IHandshakeToken> tokenCryptoService, IAccountService accountService, IIDFacade idFacade)
        {
            _accountService = accountService;
            _handshakeTokenCryptoService = tokenCryptoService;
            _idFacade = idFacade;
        }

		public Peer(ITokenCryptoService<IInfoRequestToken> tokenCryptoService, IAccountService accountService, IIDFacade idFacade)
		{
			_accountService = accountService;
			_infoRequestTokenCryptoService = tokenCryptoService;
			_idFacade = idFacade;
		}

        /// <summary>
        /// Compare public address derived from the public key used to encrypt 
        /// the token with the public address used to control the ID specify
        /// in the token header. If they match then the person sending/encrypting
        /// the token is the same as the one controlling the ID specified in
        /// the header.
        /// </summary>
        /// <param name="handshakeToken">Handshake token.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        protected void VerifyHandshakeTokenIDOwnership<T>(T handshakeToken) where T : HandshakeToken, new()
        {
            string tokenPublicAddress = AccountService.GeneratePublicAddressFromPublicKey(handshakeToken.PublicKey);

            if (tokenPublicAddress != handshakeToken.Id.Owner)
                throw new TokenSenderIsNotIDOwnerException();
        }

        /// <summary>
        /// Decrypt and parses a JSON-encoded HandshakeToken. Also verifies that the
        /// token was encrypted by the ID that issued the token.
        /// </summary>
        /// <param name="encryptedHandshakeToken">The JSON-encoded SYNACK</param>
        /// <returns>The HandshakeToken object</returns>
        protected virtual async Task<T> ProcessEncryptedHandshakeToken<T>(string encryptedHandshakeToken) where T : HandshakeToken, new()
        {
            string decryptedToken = _handshakeTokenCryptoService.Decrypt(encryptedHandshakeToken, _accountService.GetPrivateKeyAsByteArray());
            HandshakeTokenFactory<T> handshakeTokenFactory = new HandshakeTokenFactory<T>(_idFacade);
            T handshakeToken = await handshakeTokenFactory.CreateHandshakeTokenAsync(decryptedToken);

            return handshakeToken;
        }

		/// <summary>
		/// Decrypt and parses a JSON-encoded InfoRequestToken. Also verifies that the
		/// token was encrypted by the ID that issued the token.
		/// </summary>
		/// <param name="encryptedInfoRequestToken">The JSON-encoded SYNACK</param>
		/// <returns>The InfoRequestToken object</returns>
		protected virtual InfoRequestToken ProcessEncryptedInfoRequestToken<T>(string encryptedInfoRequestToken) where T : InfoRequestToken, new()
		{
			string decryptedToken = _infoRequestTokenCryptoService.Decrypt(encryptedInfoRequestToken, _accountService.GetPrivateKeyAsByteArray());
			InfoRequestTokenFactory<T> infoRequestTokenFactory = new InfoRequestTokenFactory<T>(_idFacade);
			InfoRequestToken infoRequestToken = infoRequestTokenFactory.CreateInfoRequestTokenAsync(decryptedToken);

			return infoRequestToken;
		}

		/// <summary>
		/// Compare public address derived from the public key used to encrypt 
		/// the token with the public address used to control the ID specified
		/// in the token header. If they match then the person sending/encrypting
		/// the token is the same as the one controlling the ID specified in
		/// the header.
		/// </summary>
		/// <param name="infoRequestToken">Info request token.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		protected void VerifyInfoRequestTokenIDOwnership<T>(T infoRequestToken) where T : InfoRequestToken, new()
		{
			string tokenPublicAddress = AccountService.GeneratePublicAddressFromPublicKey(infoRequestToken.PublicKey);

			if (tokenPublicAddress != infoRequestToken.Id.Owner)
				throw new TokenSenderIsNotIDOwnerException();
		}
    }
}
