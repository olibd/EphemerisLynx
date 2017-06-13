using System.Collections.Generic;
using Lynx.Core.Models.IDSubsystem;
using System.Threading.Tasks;

namespace Lynx.Core.Facade.Interfaces
{
    interface IAttributeFacade
    {
        /// <summary>
        /// Creates a new attribute
        /// </summary>
        /// <param name="attribute">The attribute to be deployed. The address fields will be updated with the smart contract addresses.</param>
        Task<Attribute> DeployAsync(Attribute attribute);

        /// <summary>
        /// Returns a new Attribute using the data contained in the contract at the address specified.
        /// </summary>
        /// <param name="address">The Attribute contract address</param>
        /// <returns>The Attribute at this address</returns>
        Task<Attribute> GetAttributeAsync(string address);

        /// <summary>
        /// Gets the list of certificates in the attribute
        /// </summary>
        /// <returns>Certificates</returns>
        Task<Dictionary<string, Certificate>> GetCertificatesAsync(Attribute attribute);

        /// <summary>
        /// Adds the certificate to the Attribute
        /// </summary>
        /// <param name="key">The key under which to add the certificate</param>
        /// <param name="cert">The certificate to add</param>
        Task<Certificate> AddCertificateAsync(Attribute attribute, Certificate cert);
    }


}