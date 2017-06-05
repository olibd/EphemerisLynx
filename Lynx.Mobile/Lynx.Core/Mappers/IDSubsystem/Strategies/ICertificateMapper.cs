using System;
using Lynx.Core.Models.IDSubsystem;

namespace Lynx.Core.Mappers.IDSubsystem.Strategies
{
    public interface ICertificateMapper<T>
    {
        /// <summary>
        /// Save the specified Certificate.
        /// </summary>
        /// <returns>The save cert key</returns>
        /// <param name="cert">The certificate object</param>
        string Save(Certificate<T> cert);

        /// <summary>
        /// Get the Certificate by its hash.
        /// </summary>
        /// <returns>ID object</returns>
        /// <param name="hash">Hash of the Certificate (acts as primary key)</param>
        Certificate<T> Get(string hash);
    }
}
