using System;
using System.Threading.Tasks;
using Lynx.Core.Facade.Interfaces;
using Lynx.Core.Models.IDSubsystem;
using Newtonsoft.Json;

namespace Lynx.Core.Communications.Packets
{
    public class CertificationConfirmationTokenFactory : TokenFactory<CertificationConfirmationToken>
    {
        private ICertificateFacade _certFacade;
        public CertificationConfirmationTokenFactory(ICertificateFacade certFacade)
        {
            _certFacade = certFacade;
        }

        public async Task<CertificationConfirmationToken> CreateTokenAsync(string encodedToken)
        {
            //decode header to get the ID address and the accessible attribtue
            string[] splittedEncodedToken = encodedToken.Split('.');

            CertificationConfirmationToken t = base.CreateToken(encodedToken);

            /////////////////////////////////////////////////////////////////////////////
            //Instantiate the Certificate array from the addresses found in the payload//
            /////////////////////////////////////////////////////////////////////////////

            //get the issued certificates                
            string[] issuedCertAddresses = JsonConvert.DeserializeObject<string[]>(t.GetFromPayload("issCert"));
            Certificate[] issuedCerts = new Certificate[issuedCertAddresses.Length];

            //load them from the blockchain
            int i = 0;
            foreach (string address in issuedCertAddresses)
            {
                issuedCerts[i] = await _certFacade.GetCertificateAsync(address);
                i++;
            }

            t.IssuedCertificates = issuedCerts;

            return t;
        }
    }
}
