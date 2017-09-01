using System;

namespace Lynx.Core.PeerVerification
{
    internal class InvalidTokenType : Exception
    {
        public InvalidTokenType()
        {
        }

        public InvalidTokenType(string message) : base(message)
        {
        }

        public InvalidTokenType(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}