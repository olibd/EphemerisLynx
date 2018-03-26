using System;
using System.Threading.Tasks;
using Nethereum.Web3;

namespace Lynx.Core.Networking
{
    public class EthBlockchainPollingService : IPollingService
    {
        private Web3 _web3;

        public EthBlockchainPollingService(Web3 web3)
        {
            _web3 = web3;
        }

        public event EventHandler<EventArgs> PollingFailure;
        public event EventHandler<EventArgs> PollingSuccess;

        public async Task<bool> Poll()
        {
            bool success;

            try
            {
                success = await _web3.Net.Listening.SendRequestAsync();
            }
            catch (Exception) { success = false; }

            if (success)
            {
                if (PollingSuccess != null)
                    PollingSuccess.Invoke(this, null);
                return true;
            }
            else
            {
                if (PollingFailure != null)
                    PollingFailure.Invoke(this, null);
                return false;
            }
        }
    }
}
