using Lynx.Core.Models.IDSubsystem;
using System.Threading.Tasks;

namespace Lynx.Core.Facade.Interfaces
{

    interface ICertificateFacade<T>
    {
        /// <summary>
        /// Returns a new Certificate using the data contained in the contract at the address specified.
        /// </summary>
        /// <param name="address">The certificate contract address</param>
        /// <returns>The certificate at this address</returns>
        Task<Certificate<T>> GetCertificateAsync(string address);

        /// <summary>
        /// Deploys the certificate to the blockchain
        /// </summary>
        /// <param name="cert">The certificate to be deployed</param>
        /// <returns>The updated certificate object (containing the address)</returns>
        Task<Certificate<T>> DeployAsync(Certificate<T> cert);
    }


}