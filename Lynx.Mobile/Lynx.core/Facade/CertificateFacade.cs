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

namespace Lynx.Core.Facade
{
    public class CertificateFacade : Facade, ICertificateFacade
    {
        private IContentService _contentService;

        public CertificateFacade(Web3 web3, IContentService contentService, IAccountService accountService) : base(web3, accountService)
        {
            _contentService = contentService;
        }

        public CertificateFacade(IContentService contentService, IAccountService accountService) : base(new Web3(), accountService)
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
            };

            certificateModel.Content = _contentService.GetContent(certificateModel.Location, certificateModel.Hash);

            return certificateModel;
        }

        public async Task<Certificate> DeployAsync(Certificate cert)
        {
            //Standard Ethereum contract deploy and get address
            string transactionHash = await CertificateService.DeployContractAsync(Web3, AccountService.PrivateKey, cert.Location, cert.Hash, cert.OwningAttribute.Address);
            TransactionReceipt receipt = await Web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);
            cert.Address = receipt.ContractAddress;
            return cert;
        }
    }
}