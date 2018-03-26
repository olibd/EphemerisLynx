namespace Lynx.Core
{
    interface ILynxConfigurationService
    {
        void ConfigureEthNode(string factoryContractAddress, string rpcEndpointUrl);
        void ConfigurePolling();
    }
}