using System;
using System.Collections.Generic;
using SQLiteNetExtensions.Attributes;

namespace Lynx.Core.Models.IDSubsystem
{
    public class Attribute : ExternalElement
    {
        [ForeignKey(typeof(ID))]
        private int IDMUID { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public Dictionary<string, Certificate> Certificates { get; set; }

        public void AddCertificate(Certificate attr)
        {
            Certificates.Add(attr.Hash, attr);
        }

        public Certificate GetCertificate(string hash)
        {
            return Certificates[hash];
        }
    }
}
