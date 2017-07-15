using System;

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
        /// Event fires whenever a message is received from the other peer. The argument is the received message.
        /// </summary>
        event EventHandler<string> OnMessageReceived;
    }
}
