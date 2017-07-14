namespace Lynx.Core.Facade.Interfaces
{
    interface ISyn : IHandshakePayload
    {
        /// <summary>
        /// The address allowing the verifier to connect back to the requester
        /// </summary>
        string NetworkAddress { get; set; }
    }
}