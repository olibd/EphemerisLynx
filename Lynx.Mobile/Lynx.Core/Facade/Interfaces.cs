using System.Collections.Generic;

namespace Lynx.Core.Facade
{
    interface IIDFacade
    {

        /// <summary>
        /// Initialize the ID Facade and prepare it to fetch data from the ID at the specified address
        /// </summary>
        /// <param name="IDAddress">The ID contract address</param>
        void Initialize(string IDAddress);

        /// <summary>
        /// Create a new ID Contract and initialize the facade to interact with it.
        /// </summary>
        /// <returns>The new ID contract's address</returns>
        string Create();

        /// <summary>
        /// Gets the list of attribute keys in the ID contract
        /// </summary>
        List<string> GetAttributeKeys(); 

        /// <summary>
        /// Gets the list of attribute addresses associated with the ID contract
        /// </summary>
        List<string> GetAttributeAddresses();

        /// <summary>
        /// Gets the address of a spefific attribute contract
        /// </summary>
        /// <param name="key">The attribute's key</param>
        /// <returns>The attribute's address, or null if there is no attribute associated with the specified key.</returns>
        string GetAttributeAddress(string key);

        /// <summary>
        /// Adds the attribute at the specified address to the ID
        /// </summary>
        /// <param name="AttributeAddress">The attribute's address</param>
        /// <param name="AttributeKey">The attribute's key</param>
        /// <returns>True if adding was successful</returns>
        bool AddAttribute(string AttributeAddress, string AttributeKey);

        /// <summary>
        /// Deletes the specified attribute from the ID
        /// </summary>
        /// <param name="AttributeKey">The attribute's key</param>
        /// <returns>True if removing was successful</returns>
        bool RemoveAttribute(string AttributeKey);
    }

    interface IAttributeFacade
    {

        /// <summary>
        /// Initialize the Attribute Facade and prepare it to fetch data from the ID at the specified address
        /// </summary>
        /// <param name="AttributeAddress">The attribute contract address</param>
        void Initialize(string AttributeAddress);

        /// <summary>
        /// Create a new Attribute Contract and initialize the facade to interact with it.
        /// </summary>
        /// <param name="content">The attribute contract's content (to be hosted)</param>
        /// <returns>The new Attribute contract's address</returns>
        void Create(AttributeContent content);

        /// <summary>
        /// Gets the list of certificate keys in the attribute
        /// </summary>
        /// <returns>certificate keys</returns>
        List<string> GetCertificateKeys();

        /// <summary>
        /// Gets the list of certificate addresses in the attribute
        /// </summary>
        /// <returns>certificate addresses</returns>
        List<string> GetCertificateAddresses();

        /// <summary>
        /// Gets the address fot the certificate associated with the specified key
        /// </summary>
        /// <param name="certificateKey">The key</param>
        /// <returns>The certificate contract's address</returns>
        string GetCertificateAddress(string certificateKey);

        /// <summary>
        /// Fetches the attribute's content from the external hosting service and verifies it against the hash stored in the contract. Throws a HashMismatchException (TODO) in case of a failure.
        /// </summary>
        /// <returns>The attribute's content</returns>
        AttributeContent GetContent();
    }

    interface ICertificateFacade
    {

        /// <summary>
        /// Initialize the Certificate Facade and prepare it to fetch data from the cert at the specified address
        /// </summary>
        /// <param name="CertificateAddress">The certificate contract address</param>
        void Initialize(string CertificateAddress);

        /// <summary>
        /// Fetches the certificate's content from the external hosting and verifies it against the hash stored in the contract. Throws a HashMismatchException (TODO) in case of a failure.
        /// </summary>
        /// <returns>The certificate content.</returns>
        string GetContent(); 

        /// <summary>
        /// Checks the certificate's revocation status.
        /// </summary>
        /// <returns>True if the contract is revoked</returns>
        bool IsRevoked();

    }


}