using Lynx.Core.Facade;
using Lynx.Core.Facade.Interfaces;
using MvvmCross.Platform;
using Nethereum.Web3;

namespace Lynx.Core
{
    class BlockchainFacadeSetup
    {
        /// <summary>
        /// Registers the Facades as singletons, creating them using the specified parameters.
        /// This should be called once the app is logged in and ready to talk to Ethereum.
        /// </summary>
        /// <param name="userAddress">The user's address (used to perform all transactions)</param>
        /// <param name="userPassword">The user's password (TODO, should used to unlock the account)</param>
        /// <param name="factoryContract">The IDFactory smart contract address, used when deploying a new ID</param>
        /// <param name="rpcEndpoint">The URL for the Ethereum node's RPC endpoint</param>
        public void CreateFacades(string userAddress, string userPassword, string factoryContract, string rpcEndpoint = "http://localhost:8545")
        {
            Web3 web3 = new Web3(rpcEndpoint);
            Mvx.RegisterSingleton<ICertificateFacade>(() => new CertificateFacade("", "", web3));
            Mvx.RegisterSingleton<IAttributeFacade>(() => new AttributeFacade("", "", web3, Mvx.Resolve<ICertificateFacade>()));
            Mvx.RegisterSingleton<IIDFacade>(() => new IDFacade("", "", "", web3, Mvx.Resolve<IAttributeFacade>()));
        }
    }
}
