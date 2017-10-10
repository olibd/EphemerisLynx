namespace Lynx.API.Models
{
    public class Session
    {
        public long Id { get; set; }
        public string CallbackEndpoint { get; set; }
        public Client Client { get; set; }
    }
}
