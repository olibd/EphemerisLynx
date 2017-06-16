using System;
using SQLite;

namespace Lynx.Core.Models.IDSubsystem
{
    public class SmartContract : Model
    {
        [Indexed]
        public string Address { get; set; }
    }
}
