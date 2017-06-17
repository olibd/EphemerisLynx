using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nethereum.Web3;
using eVi.abi.lib.pcl;
using Lynx.Core.Facade.Interfaces;
using Lynx.Core.Models.IDSubsystem;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;

namespace Lynx.Core.Facade
{
    public class AttributeFacade : Facade, IAttributeFacade
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
            string transactionHash = await AttributeService.DeployContractAsync(_web3, _address, attribute.Location, attribute.Hash, _address, new HexBigInteger(800000));
            TransactionReceipt receipt = await _web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);

            //Populating new attribute model with the new address and values passed
            outAttribute.Address = receipt.ContractAddress;
            outAttribute.Content = attribute.Content;
            outAttribute.Hash = attribute.Hash;
            outAttribute.Location = attribute.Location;

            //Iterating over certificates and deploying each one
            foreach (string key in attribute.Certificates.Keys)
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
            Attribute attributeModel = new Attribute
            {
                Address = address,
                Hash = await ethAttribute.HashAsyncCall(),
                Location = await ethAttribute.LocationAsyncCall()
            };

            //Populating attribute object with values from the smart contract

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
