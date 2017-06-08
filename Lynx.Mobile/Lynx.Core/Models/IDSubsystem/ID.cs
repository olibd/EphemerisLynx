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

        public void AddAttribute<T>(Attribute<T> attr)
        {
            Attribute<Object> attributeToAdd = (Attribute<Object>)Convert.ChangeType(attr, typeof(Attribute<Object>));
            _attributes.Add(attr.Hash, attributeToAdd);
        }

        public Attribute<T> GetAttribute<T>(string hash)
        {
            if (_attributes[hash] is Attribute<T>)
            {
                return (Attribute<T>)Convert.ChangeType(_attributes[hash], typeof(Attribute<T>));
            }
            else return null;
        }

        public Dictionary<string, Attribute<Object>>.KeyCollection GetAttributeKeys()
        {
            return _attributes.Keys;
        }
    }
}
