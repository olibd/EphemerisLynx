using System;
using Lynx.Core.Models.IDSubsystem;

namespace Lynx.Core.Mappers.IDSubsystem.Strategies
{
    public interface IAttributeMapper<T>
    {
        /// <summary>
        /// Save the specified Attribute.
        /// </summary>
        /// <returns>The save attribute key</returns>
        /// <param name="attr">The Attribute Object</param>
        string Save(Attribute<T> attr);

        /// <summary>
        /// Get the Attribute by its hash.
        /// </summary>
        /// <returns>ID object</returns>
        /// <param name="hash">Hash of the attibute (acts as primary key)</param>
        Attribute<T> Get(string hash);
    }
}
