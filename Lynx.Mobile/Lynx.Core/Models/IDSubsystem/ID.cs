using System;
using System.Collections.Generic;

namespace Lynx.Core.Models.IDSubsystem
{
    public class ID : Watchdog
    {
        public Dictionary<string, Attribute> Attributes { get; set; }

        public ID()
        {
            Attributes = new Dictionary<string, Attribute>();
        }

        public void AddAttribute(string type, Attribute attr)
        {
            Attributes.Add(type, attr);
        }

        public Attribute GetAttribute(string hash)
        {
            return Attributes[hash];
        }
    }
}
