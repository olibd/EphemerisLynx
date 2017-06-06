using System;
using Lynx.Core.Facade.Interfaces;
using Lynx.Core.Models.IDSubsystem;
using Nethereum.Web3;
using eVi.abi.lib.pcl;
using System.Threading.Tasks;

namespace Lynx.Core.Facade
{
    internal class CertificateFacade : ICertificateFacade
    {
        private Web3 _web3;

        public CertificateFacade(Web3 web3)
        {
            _web3 = web3;
        }

        public Certificate<object> GetCertificate(string address)
        {
            Task<Certificate<object>> task = GetCertificateAsync(address);
            task.Wait();
            return task.Result;
        }

        private async Task<Certificate<object>> GetCertificateAsync(string address)
        {
            CertificateService ethCertificate = new CertificateService(_web3, address);
            Certificate<object> certificateModel= new Certificate<object>();
            ICertificateFacade certFacade = new CertificateFacade(_web3);

            certificateModel.Hash = await ethCertificate.HashAsyncCall();
            certificateModel.Location = await ethCertificate.LocationAsyncCall();
            certificateModel.Revoked = await ethCertificate.RevokedAsyncCall();

            return certificateModel;
        }
    }
}