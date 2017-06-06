using System.Collections.Generic;
using Lynx.Core.Models.IDSubsystem;

namespace Lynx.Core.Facade.Interfaces
{
    interface IAttributeFacade
    {
        /// <summary>
        /// Creates a new attribute
        /// </summary>
        /// <param name="attribute">The attribute to be deployed. The address fields will be updated with the smart contract addresses.</param>
        void Deploy(out Attribute<object> attribute);

        /// <summary>
        /// Returns a new Attribute using the data contained in the contract at the address specified.
        /// </summary>
        /// <param name="address">The Attribute contract address</param>
        /// <returns>The Attribute at this address</returns>
        Attribute<object> GetAttribute(string address);

        /// <summary>
        /// Gets the list of certificates in the attribute
        /// </summary>
        /// <returns>Certificates</returns>
        Dictionary<string, Certificate<object>> GetCertificates(Attribute<object> attribute);
    }


}