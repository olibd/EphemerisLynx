using System;
using System.Collections.Generic;

namespace Lynx.Core.Services.Interfaces
{
    public interface IToken
    {
        Dictionary<string, string> Header { get; set; }
        Dictionary<string, string> Payload { get; set; }
        string GetEncodedToken { get; }
        string Signature { get; set; }
    }
}