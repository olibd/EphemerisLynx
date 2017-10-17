using System;
namespace Lynx.API.Worker.DTO
{
    /// <summary>
    /// Info request job dto. This DTO is used to restore an info request job
    /// it stores the information about a suspended job.
    /// </summary>
    public class InfoRequestJobDTO
    {
        public string SessionID { get; set; }
        public string ClientID { get; set; }
        public string EncodedSyn { get; set; }
    }
}
