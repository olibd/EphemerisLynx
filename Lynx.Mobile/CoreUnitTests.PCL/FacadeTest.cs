using System.Threading;
using System.Threading.Tasks;
using Lynx.Core;
using Lynx.Core.Facade;
using Nethereum.Web3;
using NUnit.Framework;

namespace CoreUnitTests.PCL
{
    public class FacadeTest
    {
        protected Web3 _web3;
        protected AccountService _accountService;

        protected async Task SetupAsync()
        {
            _web3 = new Web3("http://31c67be3.ngrok.io/");
            _accountService = new AccountService();
        }

    }
}