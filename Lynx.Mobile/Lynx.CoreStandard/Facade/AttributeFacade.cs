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
using Lynx.Core.Crypto.Interfaces;
using Lynx.Core.Interfaces;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Org.BouncyCastle.Crypto.Engines;
using Attribute = Lynx.Core.Models.IDSubsystem.Attribute;
using Nethereum.ABI.Encoders;
using Lynx.Core.PeerVerification;

namespace Lynx.Core.Facade
{
    public class AttributeFacade : Facade, IAttributeFacade
    {
        private ICertificateFacade _certificateFacade;
        private IContentService _contentService;

        public AttributeFacade(Web3 web3, ICertificateFacade certificateFacade, IContentService contentService, IAccountService accountService) : base(web3, accountService)
        {
            _certificateFacade = certificateFacade;
            _contentService = contentService;
        }

        public async Task<Attribute> DeployAsync(Attribute attribute, string owner)
        {
            Bytes32TypeEncoder encoder = new Bytes32TypeEncoder();

            string transactionHash = await AttributeService.DeployContractAsync(Web3, AccountService.PrivateKey, attribute.Location, encoder.Encode(attribute.Description), attribute.Hash, owner);

            TransactionReceipt receipt = null;
            try
            {
                receipt = await Web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);
            }
            catch (Exception e)
            {
                throw new FailedBlockchainDataAcess("Unable to recover the deployed attribute.", e);
            }

            //Populating the attribute model with the new address
            attribute.Address = receipt.ContractAddress;
            attribute.Owner = owner;

            //Iterating over certificates and deploying each one
            foreach (string key in attribute.Certificates.Keys)
            {
                Certificate cert = await _certificateFacade.DeployAsync(attribute.Certificates[key]);
                await AddCertificateAsync(attribute, cert);
            }

            return attribute;
        }

        public async Task<Attribute> GetAttributeAsync(string address)
        {
            AttributeService ethAttribute = new AttributeService(Web3, AccountService.PrivateKey, address);

            byte[] descriptionArr = await ethAttribute.DescriptionAsyncCall();
            string description = Encoding.UTF8.GetString(descriptionArr, 0, descriptionArr.Length);
            description = description.TrimEnd('\0');//remove null characters at the end of string

            //Populating attribute object with values from the smart contract
            Attribute attributeModel = new Attribute
            {
                Address = address,
                Hash = await ethAttribute.HashAsyncCall(),
                Location = await ethAttribute.LocationAsyncCall(),
                Owner = await ethAttribute.OwnerAsyncCall(),
                Description = description
            };

            //Fetch the content of the attribute
            attributeModel.Content = _contentService.GetContent(attributeModel.Location, attributeModel.Hash);

            //Fetching each certificate and adding them to the attribute
            Dictionary<string, Certificate> certificates = await GetCertificatesAsync(attributeModel);
            foreach (Certificate cert in certificates.Values)
            {
                //replace the OwningAttribute placeholder with a complete instance
                cert.OwningAttribute = attributeModel;
                attributeModel.AddCertificate(cert);
            }


            return attributeModel;
        }


        public async Task<Dictionary<string, Certificate>> GetCertificatesAsync(Attribute attribute)
        {
            Dictionary<string, Certificate> certs = new Dictionary<string, Certificate>();
            AttributeService ethAttribute = new AttributeService(Web3, AccountService.PrivateKey, attribute.Address);

            //Getting the number of certificates in the attribute
            BigInteger certCount = await ethAttribute.CertificateCountAsyncCall();

            //Getting each certificate and adding it to the returned dictionary
            for (BigInteger i = new BigInteger(0); i < certCount; i++)
            {
                string certKey = await ethAttribute.CertificateKeysAsyncCall(i);
                string certAddress = await ethAttribute.CertificatesAsyncCall(certKey);

                Certificate cert = await _certificateFacade.GetCertificateAsync(certAddress);
                certs.Add(certKey, cert);
            }
            return certs;
        }

        public async Task<Certificate> AddCertificateAsync(Attribute attribute, Certificate cert)
        {
            //If the certificate is not deployed, deploy it
            if (cert.Address == null)
            {
                cert = await _certificateFacade.DeployAsync(cert);
            }

            //Add the certificate to the attribute
            AttributeService ethAttribute = new AttributeService(Web3, AccountService.PrivateKey, attribute.Address);

            await ethAttribute.AddCertificateAsync(cert.Address);

            return cert;
        }
    }
}
