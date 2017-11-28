using System;

namespace Lynx.Core.Facade
{
    internal class FailedToReadBlockchainData : UserFacingException
    {
        public FailedToReadBlockchainData()
        {
        }

        public FailedToReadBlockchainData(string message) : base(message)
        {
        }

        public FailedToReadBlockchainData(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}