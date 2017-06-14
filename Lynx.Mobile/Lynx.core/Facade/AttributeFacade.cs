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
    class AttributeFacade : Facade, IAttributeFacade
    {
        private ICertificateFacade _certificateFacade;

        public AttributeFacade(string address, string password, ICertificateFacade certificateFacade) : base(address, password, new Web3())
        {
            _certificateFacade = certificateFacade;
        }

        public AttributeFacade(string address, string password, Web3 web3, ICertificateFacade certificateFacade) : base(address, password, web3)
        {
            _certificateFacade = certificateFacade;
        }

        public async Task<Attribute> DeployAsync(Attribute attribute)
        {
            Attribute outAttribute = new Attribute();
            string ethAttributeAddress = await AttributeService.DeployContractAsync(_web3, _address, attribute.Location, attribute.Hash, _address);
            AttributeService ethAttribute = new AttributeService(_web3, ethAttributeAddress);

            //Populating new attribute model with the new address and values passed
            outAttribute.Address = ethAttributeAddress;
            outAttribute.Content = attribute.Content;
            outAttribute.Hash = attribute.Hash;
            outAttribute.Location = attribute.Location;

            //Iterating over certificates and deploying each one
            foreach (string key in attribute.GetKeys())
            {
                Certificate cert = await _certificateFacade.DeployAsync(attribute.GetCertificate(key));
                cert = await AddCertificateAsync(attribute, attribute.GetCertificate(key));

                //Adding the newly deployed certificate (with updated address) to the new attribute
                outAttribute.AddCertificate(cert);
            }

            return outAttribute;
        }

        public async Task<Attribute> GetAttributeAsync(string address)
        {

            AttributeService ethAttribute = new AttributeService(_web3, address);
            Attribute attributeModel = new Attribute();

            //Populating attribute object with values from the smart contract
            attributeModel.Address = address;
            attributeModel.Hash = await ethAttribute.HashAsyncCall();
            attributeModel.Location = await ethAttribute.LocationAsyncCall();

            //Fetching each certificate and adding them to the attribute
            Dictionary<string, Certificate> certificates = await GetCertificatesAsync(attributeModel);
            foreach (Certificate cert in certificates.Values)
            {
                attributeModel.AddCertificate(cert);
            }

            //TODO: Fetch content from location and create Content object

            return attributeModel;
        }


        public async Task<Dictionary<string, Certificate>> GetCertificatesAsync(Attribute attribute)
        {
            ICertificateFacade certFacade = new CertificateFacade(_address, _password, _web3);
            Dictionary<string, Certificate> certs = new Dictionary<string, Certificate>();
            AttributeService ethAttribute = new AttributeService(_web3, attribute.Address);
            //Getting the amount of certificates in the attribute
            BigInteger certCount = await ethAttribute.CertificateCountAsyncCall();

            //Getting each certificate and adding it to the returned dictionary
            for (BigInteger i = new BigInteger(0); i < certCount; i++)
            {
                string certKey = await ethAttribute.CertificateKeysAsyncCall(i);
                string certAddress = await ethAttribute.CertificatesAsyncCall(certKey);

                Certificate cert = await certFacade.GetCertificateAsync(certAddress);
                certs.Add(certKey, cert);
            }
            return certs;
        }

        public async Task<Certificate> AddCertificateAsync(Attribute attribute, Certificate cert)
        {
            //If the certificate is not deployed, deploy it
            if (cert.Address == null)
            {
                ICertificateFacade certFacade = new CertificateFacade(_address, _password, _web3);
                cert = await certFacade.DeployAsync(cert);
            }

            //Add the certificate to the attribute
            AttributeService ethAttribute = new AttributeService(_web3, attribute.Address);
            await ethAttribute.AddCertificateAsync(_address, cert.Address);

            return cert;
        }
    }
}
