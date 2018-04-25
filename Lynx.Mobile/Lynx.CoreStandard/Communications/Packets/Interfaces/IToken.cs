namespace Lynx.Core.Communications.Packets.Interfaces
{
    public interface IToken
    {
        void SetOnHeader(string key, string val);
        void SetOnPayload(string key, string val);
        string GetFromHeader(string key);
        string GetFromPayload(string key);
        bool HeaderContains(string key);
        bool PayloadContains(string key);
        void RemoveFromHeader(string key);
        void RemoveFromPayload(string key);
        string GetEncodedHeader();
        string GetTypedEncodedHeader();
        string GetEncodedPayload();
        string GetUnsignedEncodedToken();
        string GetEncodedToken();
        /// <summary>
        /// The public key of the user sending the handshake
        /// </summary>
        string PublicKey { get; set; }
        string Signature { get; }
        void SignAndLock(string signature);
        bool Locked { get; }
        /// <summary>
        /// Specifies if the Token was/will be encrypted or not
        /// </summary>
        bool Encrypted { get; set; }
    }
}