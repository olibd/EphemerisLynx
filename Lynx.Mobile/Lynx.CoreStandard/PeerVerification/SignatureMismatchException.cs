using System;

namespace Lynx.Core.Crypto
{
    internal class SignatureMismatchException : UserFacingException
    {
        public SignatureMismatchException()
        {
        }

        public SignatureMismatchException(string message) : base(message)
        {
        }

        public SignatureMismatchException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}