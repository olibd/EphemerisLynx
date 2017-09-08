using System.Threading;
using System.Threading.Tasks;
using Lynx.Core.Facade;
using Nethereum.Web3;
using NUnit.Framework;

namespace CoreUnitTests.PCL
{
    public class FacadeTest
    {
        protected Web3 _web3;
        protected string _addressFrom;

        protected async Task SetupAsync()
        {
            _web3 = new Web3("http://f37afeb3.ngrok.io");

            _addressFrom = (await _web3.Eth.Accounts.SendRequestAsync())[0];
        }

    }
}