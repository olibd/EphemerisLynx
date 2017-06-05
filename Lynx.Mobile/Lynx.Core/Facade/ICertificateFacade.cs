namespace Lynx.Core.Facade
{

    interface ICertificateFacade
    {
        /// <summary>
        /// Returns a new CertificateModel using the data contained in the contract at the address specified.
        /// </summary>
        /// <param name="address">The certificate contract address</param>
        /// <returns>The certificate at this address</returns>
        CertificateModel GetCertificate(string address);
    }


}