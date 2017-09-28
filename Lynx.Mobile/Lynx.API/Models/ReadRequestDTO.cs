using System;
namespace Lynx.API.Models
{
    public class ReadRequestDTO
    {
        public string APIKey { get; set; }
        public string CallbackEndpoint { get; set; }
    }
}
