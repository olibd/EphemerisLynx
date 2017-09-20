using System.Collections.Generic;
using Lynx.Core.Models.IDSubsystem;
using System.Threading.Tasks;

namespace Lynx.Core.Facade.Interfaces
{
    public interface IAttributeFacade
    {
        /// <summary>
        /// Deploys an attribute to the blockchain and all attached Certificates
        /// </summary>
        /// <param name="attribute">The attribute to be deployed</param>
        /// <param name="owner">The address that will own the attribute.</param>
        /// <returns>The deployed attribute, with updated Address field (and Certificates with updated Address fields)</returns>
        Task<Attribute> DeployAsync(Attribute attribute, string owner);

        /// <summary>
        /// Fetches an attribute from the blockchain
        /// </summary>
        /// <param name="address">The address of the attribute to be fetched</param>
        /// <returns>A new attribute populated with data from the blockchain</returns>
        Task<Attribute> GetAttributeAsync(string address);

        /// <summary>
        /// Fetches the dictionary of certificates attached to an attribute
        /// </summary>
        /// <param name="attribute">The attribute to be queried</param>
        /// <returns>A dictionary representing the certificate mapping in the smart contract</returns>
        Task<Dictionary<string, Certificate>> GetCertificatesAsync(Attribute attribute);

        /// <summary>
        /// Adds a certificate to an attribute
        /// </summary>
        /// <param name="attribute">The attribute to which to add the certificate</param>
        /// <param name="cert">The certificate to add</param>
        /// <returns>The certificate with updated Address field (if necessary)</returns>
        Task<Certificate> AddCertificateAsync(Attribute attribute, Certificate cert);
    }


}