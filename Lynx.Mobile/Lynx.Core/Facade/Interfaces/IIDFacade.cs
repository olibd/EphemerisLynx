using System.Collections.Generic;
using Lynx.Core.Models.IDSubsystem;
using System.Threading.Tasks;

namespace Lynx.Core.Facade.Interfaces
{
    interface IIDFacade
    {

        /// <summary>
        /// Create a new ID contract as well as all attached attributes/models
        /// </summary>
        /// <returns>The ID, updated with the address (and all child object's addresses)</returns>
        Task<ID> DeployAsync(ID id);

        /// <summary>
        /// Populates a new ID using the data at the Ethereum address specified
        /// </summary>
        /// <param name="address">The ID contract's address</param>
        /// <returns>The ID at this address</returns>
        Task<ID> GetIDAsync(string address);

        /// <summary>
        /// Gets the attributes in the ID contract
        /// <param name="id">The ID</param>
        /// </summary>
        Task<Dictionary<byte[], Attribute>> GetAttributesAsync(ID id);

        /// <summary>
        /// Adds the attribute to the ID
        /// </summary>
        /// <param name="id">The ID to which we want to add the attribute</param>
        /// <param name="key">The key of the attribute to be added</param>
        /// <param name="attribute">The attribute to be added</param>
        /// <returns>The attribute, updated if neccesary</returns>
        Task<Attribute> AddAttributeAsync(ID id, byte[] key, Attribute attribute);
    }


}