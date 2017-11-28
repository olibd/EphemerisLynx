using System;
using System.Runtime.InteropServices;
using Lynx.Core.Facade.Interfaces;
using Lynx.Core.Models.IDSubsystem;
using Nethereum.Web3;
using eVi.abi.lib.pcl;
using System.Threading.Tasks;
using Lynx.Core.Interfaces;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Attribute = Lynx.Core.Models.IDSubsystem.Attribute;

namespace Lynx.Core.Facade
{
    public class CertificateFacade : Facade, ICertificateFacade
    {
        private IContentService _contentService;

        public CertificateFacade(Web3 web3, IContentService contentService, IAccountService accountService) : base(web3, accountService)
        {
            _contentService = contentService;
        }

        public async Task<Certificate> GetCertificateAsync(string address)
        {
            CertificateService ethCertificate = new CertificateService(Web3, AccountService.PrivateKey, address);

            //Populating certificate model with values from the smart contract
            Certificate certificateModel = new Certificate
            {
                Address = address,
                Hash = await ethCertificate.HashAsyncCall(),
                Location = await ethCertificate.LocationAsyncCall(),
                Revoked = await ethCertificate.RevokedAsyncCall(),
                Owner = await ethCertificate.OwnerAsyncCall(),
                OwningAttribute = new Attribute()
                {
                    Address = await ethCertificate.OwningAttributeAsyncCall()
                }
            };

            certificateModel.Content = _contentService.GetContent(certificateModel.Location, certificateModel.Hash);

            return certificateModel;
        }

        public async Task<Certificate> DeployAsync(Certificate cert)
        {
            //Standard Ethereum contract deploy and get address
            string transactionHash = null;
            try
            {
                transactionHash = await CertificateService.DeployContractAsync(Web3, AccountService.PrivateKey, cert.Location, cert.Hash, cert.OwningAttribute.Address);
            }
            catch (Exception e)
            {
                //TODO: log original exception
                throw new TransactionFailed("Failed to deploy the certificate for the " + cert.OwningAttribute.Description + " attribute.");
            }

            TransactionReceipt receipt;
            try
            {
                receipt = await Web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);
            }
            catch (Exception e)
            {
                //TODO: log original exception
                throw new FailedToReadBlockchainData("Unable to recover the deployed certifcate for the " + cert.OwningAttribute.Description + " attribute");
            }
            cert.Address = receipt.ContractAddress;
            cert.Owner = AccountService.GetAccountAddress();
            return cert;
        }
    }
}