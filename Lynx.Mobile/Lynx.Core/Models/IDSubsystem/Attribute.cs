using System;
using System.Collections.Generic;

namespace Lynx.Core.Models.IDSubsystem
{
    public class Attribute : ExternalElement
    {
        private Dictionary<string, Certificate> _certificates;

        public Attribute()
        {
        }

        public void AddCertificate(Certificate attr)
        {
            _certificates.Add(attr.Hash, attr);
        }

        public Certificate GetCertificate(string hash)
        {
            return _certificates[hash];
        }

        public Dictionary<string, Certificate>.KeyCollection GetKeys()
        {
            return _certificates.Keys;
        }

        
    }
}
