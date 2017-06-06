using Lynx.Core.Models.IDSubsystem;

namespace Lynx.Core.Facade.Interfaces
{

    interface ICertificateFacade
    {
        /// <summary>
        /// Returns a new Certificate using the data contained in the contract at the address specified.
        /// </summary>
        /// <param name="address">The certificate contract address</param>
        /// <returns>The certificate at this address</returns>
        Certificate<string> GetCertificate(string address);
    }


}