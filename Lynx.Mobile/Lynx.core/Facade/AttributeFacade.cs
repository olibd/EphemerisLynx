using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nethereum.Web3;
using eVi.abi.lib.pcl;
using Lynx.Core.Facade.Interfaces;
using Lynx.Core.Models.IDSubsystem;
using System.Numerics;

namespace Lynx.Core.Facade
{
    class AttributeFacade : IAttributeFacade
    {
        private Web3 _web3;


        public AttributeFacade(Web3 web3)
        {
            _web3 = web3;
        }

        public AttributeFacade()
        {
            _web3 = new Web3();
        }

        public void Deploy(out Attribute<object> attribute)
        {
            //TODO: everything
            throw new NotImplementedException();
        }

        public Attribute<object> GetAttribute(string address)
        {
            //Temporary way of handling async calls - I assume we'll make this function async at some point
            Task<Attribute<object>> task = GetAttributeAsync(address);
            task.Wait();
            return task.Result;
        }

        public Dictionary<string, Certificate<object>> GetCertificates(Attribute<object> attribute)
        {
            Task<Dictionary<string, Certificate<object>>> task = GetCertificatesAsync(attribute);
            task.Wait();
            return task.Result;
        }

        private async Task<Attribute<object>> GetAttributeAsync(string address)
        {

            AttributeService ethAttribute = new AttributeService(_web3, address);
            Attribute<object> attributeModel= new Attribute<object>();

            attributeModel.Address = address;
            attributeModel.Hash = await ethAttribute.HashAsyncCall();
            attributeModel.Location = await ethAttribute.LocationAsyncCall();

            Dictionary<string, Certificate<object>> certificates = GetCertificates(attributeModel);
            foreach(Certificate<object> cert in certificates.Values)
            {
                attributeModel.AddCertificate(cert);
            }

            return attributeModel;
            //TODO: Fetch content from location and populate generic with it
        }


        public async Task<Dictionary<string, Certificate<object>>> GetCertificatesAsync(Attribute<object> attribute)
        {
            ICertificateFacade certFacade = new CertificateFacade(_web3);
            Dictionary<string, Certificate<object>> certs = new Dictionary<string, Certificate<object>>();

            AttributeService ethAttribute = new AttributeService(_web3, attribute.Address);
            BigInteger certCount = await ethAttribute.CertificateCountAsyncCall();

            for(BigInteger i = new BigInteger(0); i < certCount; i++)
            {
                string certKey = await ethAttribute.CertificateKeysAsyncCall(i);
                string certAddress = await ethAttribute.CertificatesAsyncCall(certKey);

                Certificate<object> cert = certFacade.GetCertificate(certAddress);
                certs.Add(certKey, cert);
            }
            return certs;
        }

    }
}
