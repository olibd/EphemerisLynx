using System;
using Lynx.Core.Models.IDSubsystem;

namespace Lynx.Core.Mappers.IDSubsystem.Strategies
{
    public interface IIDMapper
    {
        /// <summary>
        /// Save the specified id.
        /// </summary>
        /// <returns>The save ID key</returns>
        /// <param name="id">ID</param>
        string Save(ID id);

        /// <summary>
        /// Get the ID by its primaryKey.
        /// </summary>
        /// <returns>ID object</returns>
        /// <param name="primaryKey">Primary key</param>
        ID Get(string primaryKey);
    }
}
