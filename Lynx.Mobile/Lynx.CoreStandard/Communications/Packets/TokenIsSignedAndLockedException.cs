using System;

namespace Lynx.Core.Communications.Packets
{
    public class TokenIsSignedAndLockedException : Exception
    {
        public TokenIsSignedAndLockedException()
        {
        }

        public TokenIsSignedAndLockedException(string message) : base(message)
        {
        }

        public TokenIsSignedAndLockedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}