namespace Lynx.Core
{
    interface ILynxConfigurationService
    {
        void ConfigureEthNode(string userAddress, string userPassword, string factoryContractAddress, string rpcEndpointUrl);
    }
}