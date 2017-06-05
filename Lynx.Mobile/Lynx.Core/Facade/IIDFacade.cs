using System.Collections.Generic;

namespace Lynx.Core.Facade
{
    interface IIDFacade
    {

        /// <summary>
        /// Create a new ID contract as well as all attached attributes/models
        /// </summary>
        /// <returns>The new ID contract's address</returns>
        string Deploy(IDModel id);

        /// <summary>
        /// Populates a new IDModel using the data at the Ethereum address specified
        /// </summary>
        /// <param name="address">The ID contract's address</param>
        /// <returns>The ID at this address</returns>
        IDModel GetID(string address);

        /// <summary>
        /// Gets the list of attributes in the ID contract
        /// </summary>
        List<AttributeModel> GetAttributes();

        /// <summary>
        /// Adds the attribute to the ID
        /// </summary>
        /// <param name="model">The attribute to be added</param>
        /// <returns>True if adding was successful</returns>
        bool AddAttribute(AttributeModel attribute);
    }


}