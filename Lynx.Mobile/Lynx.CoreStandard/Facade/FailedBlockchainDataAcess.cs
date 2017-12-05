using System;
namespace Lynx.Core.PeerVerification
{
    public class FailedBlockchainDataAcess : UserFacingException
    {
        public FailedBlockchainDataAcess()
        {
        }

        public FailedBlockchainDataAcess(string message) : base(message)
        {
        }

        public FailedBlockchainDataAcess(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
