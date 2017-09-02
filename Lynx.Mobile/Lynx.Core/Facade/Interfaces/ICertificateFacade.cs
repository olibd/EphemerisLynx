using Lynx.Core.Models.IDSubsystem;
using System.Threading.Tasks;

namespace Lynx.Core.Facade.Interfaces
{

    public interface ICertificateFacade
    {
        /// <summary>
        /// Fetches a certificate from the blockchain. The OwningAttribute
        /// property will contain a placeholder Attribute containing only
        /// the attribute's address. It is the responsability of the
        /// caller to replace it with a complete instance.
        /// </summary>
        /// <param name="address">The address of the certificate to be fetched</param>
        /// <returns>A new certificate populated with data from the blockchain</returns>
        Task<Certificate> GetCertificateAsync(string address);

        /// <summary>
        /// Deploys a certificate to the blockchain
        /// </summary>
        /// <param name="cert">The certificate to be deployed</param>
        /// <returns>The deployed certificate, with updated Address field</returns>
        Task<Certificate> DeployAsync(Certificate cert);
    }


}