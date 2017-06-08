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
    class AttributeFacade<T> : Facade, IAttributeFacade<T>
    {
        public AttributeFacade(string address, string password) : base(address, password, new Web3())
        {
        }

        public AttributeFacade(string address, string password, Web3 web3) : base(address, password, web3)
        {
        }

        public async Task<Attribute<T>> DeployAsync(Attribute<T> attribute)
        {
            Attribute<T> outAttribute = new Attribute<T>();
            ICertificateFacade<T> certFacade = new CertificateFacade<T>(_address, _password, _web3);
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
                Certificate<T> cert = await certFacade.DeployAsync(attribute.GetCertificate(key));
                cert = await AddCertificateAsync(attribute, attribute.GetCertificate(key));

                //Adding the newly deployed certificate (with updated address) to the new attribute
                outAttribute.AddCertificate(cert);
            }

            return outAttribute;
        }

        public async Task<Attribute<T>> GetAttributeAsync(string address)
        {

            AttributeService ethAttribute = new AttributeService(_web3, address);
            Attribute<T> attributeModel = new Attribute<T>();

            //Populating attribute object with values from the smart contract
            attributeModel.Address = address;
            attributeModel.Hash = await ethAttribute.HashAsyncCall();
            attributeModel.Location = await ethAttribute.LocationAsyncCall();

            //Fetching each certificate and adding them to the attribute
            Dictionary<string, Certificate<T>> certificates = await GetCertificatesAsync(attributeModel);
            foreach (Certificate<T> cert in certificates.Values)
            {
                attributeModel.AddCertificate(cert);
            }

            //TODO: Fetch content from location and populate generic with it

            return attributeModel;
        }


        public async Task<Dictionary<string, Certificate<T>>> GetCertificatesAsync(Attribute<T> attribute)
        {
            ICertificateFacade<T> certFacade = new CertificateFacade<T>(_address, _password, _web3);
            Dictionary<string, Certificate<T>> certs = new Dictionary<string, Certificate<T>>();
            AttributeService ethAttribute = new AttributeService(_web3, attribute.Address);
            //Getting the amount of certificates in the attribute
            BigInteger certCount = await ethAttribute.CertificateCountAsyncCall();

            //Getting each certificate and adding it to the returned dictionary
            for (BigInteger i = new BigInteger(0); i < certCount; i++)
            {
                string certKey = await ethAttribute.CertificateKeysAsyncCall(i);
                string certAddress = await ethAttribute.CertificatesAsyncCall(certKey);

                Certificate<T> cert = await certFacade.GetCertificateAsync(certAddress);
                certs.Add(certKey, cert);
            }
            return certs;
        }

        public async Task<Certificate<T>> AddCertificateAsync(Attribute<T> attribute, Certificate<T> cert)
        {
            //If the certificate is not deployed, deploy it
            if (cert.Address == null)
            {
                ICertificateFacade<T> certFacade = new CertificateFacade<T>(_address, _password, _web3);
                cert = await certFacade.DeployAsync(cert);
            }

            //Add the certificate to the attribute
            AttributeService ethAttribute = new AttributeService(_web3, attribute.Address);
            await ethAttribute.AddCertificateAsync(_address, cert.Address);

            return cert;
        }
    }
}
