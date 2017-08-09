namespace Lynx.Core.Services.Communications.Packets.Interfaces
{
    public interface IToken
    {
        void SetOnHeader(string key, string val);
        void SetOnPayload(string key, string val);
        string GetFromHeader(string key);
        string GetFromPayload(string key);
        string GetEncodedHeader();
        string GetEncodedPayload();
        string GetUnsignedEncodedToken();
        string GetEncodedToken();
        string Signature { get; }
        void SignAndLock(string signature);
        bool Locked { get; }
    }
}