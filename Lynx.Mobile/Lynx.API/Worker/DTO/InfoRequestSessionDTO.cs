using System;
namespace Lynx.API.Worker.DTO
{
    public class InfoRequestSessionDTO
    {
        public string SessionID { get; set; }
        public string ClientID { get; set; }
        public string EncodedSyn { get; set; }
    }
}
