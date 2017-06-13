using System;
using System.Collections.Generic;

namespace Lynx.Core.Models.IDSubsystem
{
    public class ID : Watchdog
    {

        private Dictionary<string, Attribute> _attributes;

        public ID()
        {

        }

        public void AddAttribute(Attribute attr)
        {
            _attributes.Add(attr.Hash, attr);
        }

        public Attribute GetAttribute(string hash)
        {
            return _attributes[hash];
        }

        public Dictionary<string, Attribute>.KeyCollection GetAttributeKeys()
        {
            return _attributes.Keys;
        }
    }
}
