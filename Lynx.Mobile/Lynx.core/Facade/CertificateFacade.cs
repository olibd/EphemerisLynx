using System;
using System.Runtime.InteropServices;
using Lynx.Core.Facade.Interfaces;
using Lynx.Core.Models.IDSubsystem;
using Nethereum.Web3;
using eVi.abi.lib.pcl;
using System.Threading.Tasks;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;

namespace Lynx.Core.Facade
{
    public class CertificateFacade : Facade, ICertificateFacade
    {
        private IContentService _contentService;
        public CertificateFacade(string address, string password, Web3 web3, IContentService contentService) : base(address, password, web3)
        {
            _contentService = contentService;
        }

        public CertificateFacade(string address, string password, IContentService contentService) : base(address, password, new Web3())
        {
            _contentService = contentService;
        }

        public async Task<Certificate> GetCertificateAsync(string address)
        {
            CertificateService ethCertificate = new CertificateService(Web3, address);

            //Populating certificate model with values from the smart contract
            Certificate certificateModel = new Certificate
            {
                Address = address,
                Hash = await ethCertificate.HashAsyncCall(),
                Location = await ethCertificate.LocationAsyncCall(),
                Revoked = await ethCertificate.RevokedAsyncCall(),
                Owner = await ethCertificate.OwnerAsyncCall()
            };

            certificateModel.Content = _contentService.GetContent(certificateModel.Location, certificateModel.Hash);

            return certificateModel;
        }

        public async Task<Certificate> DeployAsync(Certificate cert)
        {
            //Standard Ethereum contract deploy and get address
            string transactionHash = await CertificateService.DeployContractAsync(Web3, Address, cert.Location, cert.Hash, cert.OwningAttribute.Address, new HexBigInteger(600000));
            TransactionReceipt receipt = await Web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);
            cert.Address = receipt.ContractAddress;
            cert.Owner = Address;
            return cert;
        }
    }
}