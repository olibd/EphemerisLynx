using System;

namespace Lynx.Core.Services
{
    internal class SignatureDoesntMatchException : Exception
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