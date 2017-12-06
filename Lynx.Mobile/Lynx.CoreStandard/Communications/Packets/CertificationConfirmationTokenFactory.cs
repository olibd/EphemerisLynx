using System;
using System.Threading.Tasks;
using eVi.abi.lib.pcl;
using Lynx.Core.Facade.Interfaces;
using Lynx.Core.Models.IDSubsystem;
using Lynx.Core.PeerVerification;
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

            ///////////////////////////////////////////////////////////////////
            //If the token has a signature, then the base will return a locked
            //token, we need to extract the signature from the encoded token
            //supplied to the base. And we'll add the signature in this method
            //instead
            ///////////////////////////////////////////////////////////////////
            string signature = null;
            if (splittedEncodedToken.Length == 3)
            {
                //save a copy of the signature
                signature = splittedEncodedToken[2];
                //remove the signature from the encodedToken
                encodedToken = splittedEncodedToken[0] + "." + splittedEncodedToken[1];
            }

            CertificationConfirmationToken t = base.CreateToken(encodedToken);

            /////////////////////////////////////////////////////////////////////////////
            //Instantiate the Certificate array from the addresses found in the payload//
            /////////////////////////////////////////////////////////////////////////////

            //get the issued certificates                
            string[] issuedCertAddresses = JsonConvert.DeserializeObject<string[]>(t.GetFromPayload("issCert"));
            Certificate[] issuedCerts = new Certificate[issuedCertAddresses.Length];

            //load them from the blockchain
            int i = 0;
            try
            {
                foreach (string address in issuedCertAddresses)
                {
                    issuedCerts[i] = await _certFacade.GetCertificateAsync(address);
                    i++;
                }
            }
            catch (CallFailed e)
            {
                throw new FailedBlockchainDataAcess("Unable to recover the certificate(s) data.", e);
            }

            t.IssuedCertificates = issuedCerts;

            //If there was a signature in the encoded token, we sign it
            if (signature != null)
                t.SignAndLock(signature);

            return t;
        }
    }
}
