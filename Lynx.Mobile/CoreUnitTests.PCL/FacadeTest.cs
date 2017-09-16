using System.Threading;
using System.Threading.Tasks;
using eVi.abi.lib.pcl;
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
        protected FactoryService _factoryService;

        protected async Task SetupAsync()
        {
            _web3 = new Web3("http://jmon.tech:8545");
            _accountService = new AccountService();
            _factoryService = new FactoryService(_web3, _accountService.PrivateKey, "0x3dd0864668c36d27b53a98137764c99f9fd5b7b2");
        }

    }
}