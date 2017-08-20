using Lynx.Core.Facade;
using System.Threading.Tasks;
using Lynx.Core.Facade.Interfaces;
using MvvmCross.Platform;
using Nethereum.Web3;
using Nethereum.JsonRpc.Client;
using System;
using Lynx.Core.Interfaces;

namespace Lynx.Core
{
    class LynxConfigurationService : ILynxConfigurationService
    {
        /// <summary>
        /// Registers the Facades as singletons, creating them using the specified parameters.
        /// This should be called once the app is logged in and ready to talk to Ethereum.
        /// </summary>
        /// <param name="userAddress">The user's address (used to perform all transactions)</param>
        /// <param name="userPassword">The user's password (TODO, should used to unlock the account)</param>
        /// 
        /// <param name="factoryContract">The IDFactory smart contract address, used when deploying a new ID</param>
        /// <param name="rpcEndpoint">The URL for the Ethereum node's RPC endpoint</param>
        public void ConfigureEthNode(string userAddress, string userPassword, string factoryContract, string rpcEndpoint)
        {
            Web3 web3 = new Web3(rpcEndpoint);
            Mvx.RegisterSingleton<Web3>(() => web3);
            Mvx.RegisterSingleton<IAccountService>(() => new AccountService());
            Mvx.RegisterSingleton<IContentService>(() => new DummyContentService());
            Mvx.RegisterSingleton<ICertificateFacade>(() => new CertificateFacade(web3, Mvx.Resolve<IContentService>(), Mvx.Resolve<IAccountService>()));
            Mvx.RegisterSingleton<IAttributeFacade>(() => new AttributeFacade(web3, Mvx.Resolve<ICertificateFacade>(), Mvx.Resolve<IContentService>(), Mvx.Resolve<IAccountService>()));
            Mvx.RegisterSingleton<IIDFacade>(() => new IDFacade(factoryContract, web3, Mvx.Resolve<IAttributeFacade>(), Mvx.Resolve<IAccountService>()));
        }
    }
}
