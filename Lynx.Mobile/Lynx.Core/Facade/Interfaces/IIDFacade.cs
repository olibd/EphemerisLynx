using System.Collections.Generic;
using Lynx.Core.Models.IDSubsystem;

namespace Lynx.Core.Facade.Interfaces
{
    interface IIDFacade
    {

        /// <summary>
        /// Create a new ID contract as well as all attached attributes/models
        /// </summary>
        /// <returns>The new ID contract's address</returns>
        string Deploy(ID id);

        /// <summary>
        /// Populates a new ID using the data at the Ethereum address specified
        /// </summary>
        /// <param name="address">The ID contract's address</param>
        /// <returns>The ID at this address</returns>
        ID GetID(string address);

        /// <summary>
        /// Gets the list of attributes in the ID contract
        /// </summary>
        List<Attribute<object>> GetAttributes();

        /// <summary>
        /// Adds the attribute to the ID
        /// </summary>
        /// <param name="attribute">The attribute to be added</param>
        /// <returns>True if adding was successful</returns>
        bool AddAttribute(Attribute<object> attribute);
    }


}