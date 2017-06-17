using System.Threading;
using System.Threading.Tasks;
using Lynx.Core.Facade;
using Nethereum.TestRPCRunner;
using Nethereum.Web3;
using NUnit.Framework;

namespace CoreUnitTests
{
    public class FacadeTest
    {
        protected TestRPCEmbeddedRunner _runner;
        protected Web3 _web3;
        protected string _addressFrom;

        protected async Task SetupAsync()
        {
            _web3 = new Web3();
            _runner = new TestRPCEmbeddedRunner
            {
                RedirectOuputToDebugWindow = true,
                Arguments = "--port 8545"
            };

            _runner.StartTestRPC();
            //Give TestRPC some time
            Thread.Sleep(2000);

            _addressFrom = (await _web3.Eth.Accounts.SendRequestAsync())[0];
        }

    }
}