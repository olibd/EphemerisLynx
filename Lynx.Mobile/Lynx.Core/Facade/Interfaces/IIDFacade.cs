using System.Collections.Generic;
using Lynx.Core.Models.IDSubsystem;
using System.Threading.Tasks;

namespace Lynx.Core.Facade.Interfaces
{
    public interface IIDFacade
    {

        /// <summary>
        /// Deploys and populates ID and IDController contracts from a local ID object
        /// </summary>
        /// <param name="id">The ID to deploy</param>
        /// <returns>The ID, with any attached Attributes or Certificates, with updated Address fields</returns>
        Task<ID> DeployAsync(ID id);

		/// <summary>
		/// Fetches an ID from the blockchain
		/// </summary>
		/// <param name="address">The address of the ID to be fetched</param>
		/// <param name="accessibleAttributes">The array of keys of accessible attribute to be fetched</param>
		/// <returns>A new ID populated with data from the blockchain (and attached Attributes and Certificates)</returns>
		Task<ID> GetIDAsync(string address, string[] accessibleAttributes=null);

        /// <summary>
        /// Fetches the dictionary of attributes attached to an ID
        /// </summary>
        /// <param name="ID">The ID to be queried</param>
        /// <returns>A dictionary representing the attribute mapping in the smart contract</returns>
        Task<Dictionary<string, Attribute>> GetAttributesAsync(ID id);

		/// <summary>
		/// Fetches the dictionary of accessible attributes attached to an ID
		/// </summary>
		/// <param name="ID">The ID to be queried</param>
		/// <param name="ID">The array of keys of accessible attributes to be queried</param>
		/// <returns>A dictionary representing the attribute mapping in the smart contract</returns>
		Task<Dictionary<string, Attribute>> GetAttributesAsync(ID id, string[] accessibleAttributes);

		/// <summary>
		/// Adds an attribute to an ID
		/// </summary>
		/// <param name="id">The ID to which to add the attribute</param>
		/// <param name="key">The key under which to store the attribute</param>
		/// <param name="attribute">The attribute to add</param>
		/// <returns>The attribute with updated Address field (if necessary)</returns>
		Task<Attribute> AddAttributeAsync(ID id, byte[] key, Attribute attribute);
    }
}