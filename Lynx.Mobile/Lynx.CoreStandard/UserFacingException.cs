using System;

namespace Lynx.Core
{
    public abstract class UserFacingException : Exception
    {
        protected UserFacingException()
        {
        }

        protected UserFacingException(string message) : base(message)
        {
        }

        protected UserFacingException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
