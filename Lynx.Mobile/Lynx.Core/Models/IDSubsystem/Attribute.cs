using System;
using System.Collections.Generic;
using SQLite;

namespace Lynx.Core.Models.IDSubsystem
{
    public class Attribute : ExternalElement
    {
        [Ignore]
        public Dictionary<string, Certificate> Certificates { get; set; }

        public Attribute()
        {
            Certificates = new Dictionary<string, Certificate>();
        }

        public void AddCertificate(Certificate cert)
        {
            Certificates.Add(cert.Hash, cert);
        }

        public Certificate GetCertificate(string hash)
        {
            return Certificates[hash];
        }
    }
}
