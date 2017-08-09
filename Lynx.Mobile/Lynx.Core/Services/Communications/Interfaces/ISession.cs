using System;

namespace Lynx.Core.Services.Communications.Interfaces
{
    public interface ISession
    {
        /// <summary>
        /// Opens a session using the specified address
        /// </summary>
        /// <param name="networkAddress">The session address</param>
        void Open(string networkAddress);

        /// <summary>
        /// Opens a session and starts listening for messages
        /// </summary>
        /// <returns>The session's Network Address</returns>
        string Open();

        /// <summary>
        /// Closes the current session
        /// </summary>
        void Close();

        /// <summary>
        /// Sends a message to the other peer
        /// </summary>
        /// <param name="message">The payload to transmit</param>
        void Send(string message);

        /// <summary>
        /// Add an event handler for incoming messages
        /// </summary>
        /// <param name="handler">The event handler function - signature should be (object sender, string arg)</param>
        void AddMessageReceptionHandler(EventHandler<string> handler);
    }

}
