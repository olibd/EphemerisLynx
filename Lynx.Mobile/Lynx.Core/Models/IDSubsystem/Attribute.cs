using System;
using System.Collections.Generic;

namespace Lynx.Core.Models.IDSubsystem
{
    public class Attribute<T> : ExternalElement<T>
    {
        private Dictionary<string, Certificate<T>> _certificates;

        public Attribute()
        {
        }

        public void AddCertificate(Certificate<T> attr)
        {
            _certificates.Add(attr.Hash, attr);
        }

        public Certificate<T> GetCertificate(string hash)
        {
            return _certificates[hash];
        }

        public Dictionary<string, Certificate<T>>.KeyCollection GetKeys()
        {
            return _certificates.Keys;
        }

        
    }
}
