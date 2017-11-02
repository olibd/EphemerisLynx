namespace Lynx.Core
{
    interface ILynxConfigurationService
    {
        void ConfigureEthNode(string factoryContractAddress, string registryContractAddress, string rpcEndpointUrl);
    }
}