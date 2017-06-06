using System.Collections.Generic;

namespace Lynx.Core.Facade
{
    interface IAttributeFacade
    {
        /// <summary>
        /// Creates a new attribute
        /// </summary>
        /// <param name="attribute"></param>
        void Deploy(Attribute attribute);

        /// <summary>
        /// Returns a new Attribute using the data contained in the contract at the address specified.
        /// </summary>
        /// <param name="address">The Attribute contract address</param>
        /// <returns>The Attribute at this address</returns>
        Attribute GetAttribute(string address);

        /// <summary>
        /// Gets the list of certificates in the attribute
        /// </summary>
        /// <returns>Certificates</returns>
        List<Certificate> GetCertificates(Attribute attribute);
    }


}