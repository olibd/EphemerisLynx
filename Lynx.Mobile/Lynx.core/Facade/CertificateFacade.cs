using System;
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
        public CertificateFacade(string address, string password, Web3 web3) : base(address, password, web3)
        {
        }

        public CertificateFacade(string address, string password) : base(address, password, new Web3())
        {
        }

        public async Task<Certificate> GetCertificateAsync(string address)
        {
            CertificateService ethCertificate = new CertificateService(_web3, address);
            Certificate certificateModel = new Certificate
            {
                Hash = await ethCertificate.HashAsyncCall(),
                Location = await ethCertificate.LocationAsyncCall(),
                Revoked = await ethCertificate.RevokedAsyncCall()
            };

            //Populating certificate model with values from the smart contract


            return certificateModel;
        }

        public async Task<Certificate> DeployAsync(Certificate cert)
        {
            string transactionHash = await CertificateService.DeployContractAsync(_web3, _address, cert.Location, cert.Hash, cert.OwningAttribute.Address, new HexBigInteger(600000));
            TransactionReceipt receipt = await _web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);
            cert.Address = receipt.ContractAddress;
            return cert;
        }
    }
}