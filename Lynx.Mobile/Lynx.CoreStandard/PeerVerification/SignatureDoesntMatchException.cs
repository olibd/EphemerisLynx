using System;

namespace Lynx.Core.Crypto
{
    internal class SignatureDoesntMatchException : UserFacingException
    {
        public SignatureDoesntMatchException()
        {
        }

        public SignatureDoesntMatchException(string message) : base(message)
        {
        }

        public SignatureDoesntMatchException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}