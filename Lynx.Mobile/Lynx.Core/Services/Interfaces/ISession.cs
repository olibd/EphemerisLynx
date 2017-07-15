namespace Lynx.Core.Services.Interfaces
{
    interface ISession
    {
        /// <summary>
        /// Opens a session using the specified address
        /// </summary>
        /// <param name="networkAddress">The session address</param>
        void Open(string networkAddress);

        /// <summary>
        /// Closes the current session
        /// </summary>
        void Close();
    }
}
