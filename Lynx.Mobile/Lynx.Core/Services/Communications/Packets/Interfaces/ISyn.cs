namespace Lynx.Core.Services.Communications.Packets.Interfaces
{
    public interface ISyn : IHandshakeToken
    {
        /// <summary>
        /// The address allowing the verifier to connect back to the requester
        /// </summary>
        string NetworkAddress { get; set; }
    }
}