using System;
using System.Collections.Generic;
using SQLite;
using Attribute = Lynx.Core.Models.IDSubsystem.Attribute;

namespace Lynx.Core.Models.IDSubsystem
{
    public class ID : Watchdog
    {
        [Ignore]
        public Dictionary<string, Attribute> Attributes { get; set; }
        public string ControllerAddress { get; set; }

        public ID()
        {
            Attributes = new Dictionary<string, Attribute>();
        }

        public void AddAttribute(Attribute attr)
        {
            Attributes.Add(attr.Description, attr);
        }

        public Attribute GetAttribute(string description)
        {
            return Attributes[description];
        }
    }
}
