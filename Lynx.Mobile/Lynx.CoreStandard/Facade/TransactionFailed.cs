using System;

namespace Lynx.Core.Facade
{
    public class TransactionFailed : UserFacingException
    {
        public TransactionFailed()
        {
        }

        public TransactionFailed(string message) : base(message)
        {
        }

        public TransactionFailed(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}