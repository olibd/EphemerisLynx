using System;
using System.Collections.Generic;
using SQLite;

namespace Lynx.Core.Models.IDSubsystem
{
    public class Attribute : ExternalElement
    {
        public String Description { get; set; }

        [Ignore]
        public Dictionary<string, Certificate> Certificates { get; set; }

        public Attribute()
        {
            Certificates = new Dictionary<string, Certificate>();
        }

        public void AddCertificate(Certificate cert)
        {
            Certificates.Add(cert.Owner, cert);
        }
    }
}
