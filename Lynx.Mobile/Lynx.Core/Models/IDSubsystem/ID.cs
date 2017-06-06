using System;
using System.Collections.Generic;

namespace Lynx.Core.Models.IDSubsystem
{
    public class ID : Watchdog
    {

        private Dictionary<string, Attribute<Object>> _attributes;

        public ID()
        {

        }

        public void AddAttribute(Attribute<Object> attr)
        {
            _attributes.Add(attr.Hash, attr);
        }

        public Attribute<Object> GetAttribute(string hash)
        {
            return _attributes[hash];
        }
    }
}
