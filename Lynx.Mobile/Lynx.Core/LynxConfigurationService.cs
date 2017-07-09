using Lynx.Core.Facade;
using System.Threading.Tasks;
using Lynx.Core.Facade.Interfaces;
using MvvmCross.Platform;
using Nethereum.Web3;
using Nethereum.JsonRpc.Client;
using System;

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
            string ClientUrl = rpcEndpoint;
            RpcClient Client = new RpcClient(new Uri(ClientUrl));
            Web3 web3 = new Web3(rpcEndpoint);
            string addressFrom = (web3.Eth.Accounts.SendRequestAsync().Result)[0];

            Mvx.RegisterSingleton<IContentService>(() => new DummyContentService());
            Mvx.RegisterSingleton<ICertificateFacade>(() => new CertificateFacade(addressFrom, "", web3, Mvx.Resolve<IContentService>()));
            Mvx.RegisterSingleton<IAttributeFacade>(() => new AttributeFacade(addressFrom, "", web3, Mvx.Resolve<ICertificateFacade>(), Mvx.Resolve<IContentService>()));
            Mvx.RegisterSingleton<IIDFacade>(() => new IDFacade(addressFrom, "", factoryContract, web3, Mvx.Resolve<IAttributeFacade>()));
        }
    }
}
