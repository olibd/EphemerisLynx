using System;
using System.Collections.Generic;
using Lynx.Core.Models.IDSubsystem;
using Newtonsoft.Json;

namespace Lynx.Core.Communications.Packets
{
    public class CertificationConfirmationToken : Token
    {
        private Certificate[] _issuedCertificates;
        public Certificate[] IssuedCertificates
        {
            get
            {
                return _issuedCertificates;
            }

            set
            {
                if (value != null)
                {
                    string[] certAddresses = new string[value.Length];

                    for (int i = 0; i < value.Length; i++)
                    {
                        certAddresses[i] = value[i].Address;
                    }

                    SetOnPayload("issCert", JsonConvert.SerializeObject(certAddresses));
                    _issuedCertificates = value;
                }
                else
                {
                    RemoveFromPayload("issCert");
                    _issuedCertificates = null;
                }
            }
        }

        public CertificationConfirmationToken() : base()
        {
        }

        public CertificationConfirmationToken(Dictionary<string, string> header, Dictionary<string, string> payload) : base(header, payload)
        {
        }
    }
}
