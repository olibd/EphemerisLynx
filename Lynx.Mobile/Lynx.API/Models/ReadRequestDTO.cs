using System;
namespace Lynx.API.Models
{
    /// <summary>
    /// Read request dto. When the controllers receives a read request. The JSON
    /// body should map to this DTO structure. If it matches, it will be auto
    /// populated by the .NET Core framework
    /// </summary>
    public class ReadRequestDTO
    {
        public string APIKey { get; set; }
        public string CallbackEndpoint { get; set; }
    }
}
