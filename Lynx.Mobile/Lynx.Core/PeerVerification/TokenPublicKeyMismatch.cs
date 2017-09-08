using System;

namespace Lynx.Core.PeerVerification
{
    internal class TokenPublicKeyMismatch : Exception
    {
        public TokenPublicKeyMismatch()
        {
        }

        public TokenPublicKeyMismatch(string message) : base(message)
        {
        }

        public TokenPublicKeyMismatch(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}