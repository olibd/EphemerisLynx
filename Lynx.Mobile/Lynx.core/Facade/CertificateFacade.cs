using System;
using Lynx.Core.Facade.Interfaces;
using Lynx.Core.Models.IDSubsystem;
using Nethereum.Web3;
using eVi.abi.lib.pcl;
using System.Threading.Tasks;

namespace Lynx.Core.Facade
{
    internal class CertificateFacade : Facade, ICertificateFacade
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
            Certificate certificateModel = new Certificate();

            //Populating certificate model with values from the smart contract
            certificateModel.Hash = await ethCertificate.HashAsyncCall();
            certificateModel.Location = await ethCertificate.LocationAsyncCall();
            certificateModel.Revoked = await ethCertificate.RevokedAsyncCall();


            return certificateModel;
        }

        public async Task<Certificate> DeployAsync(Certificate cert)
        {
            string certAddress = await CertificateService.DeployContractAsync(_web3, _address, cert.Location, cert.Hash, cert.OwningAttribute.Address);
            cert.Address = certAddress;
            return cert;
        }
    }
}