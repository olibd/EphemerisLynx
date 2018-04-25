using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lynx.Core.Facade;
using Lynx.Core.Facade.Interfaces;
using Lynx.Core.Models.IDSubsystem;
using Newtonsoft.Json;

namespace Lynx.Core.Communications.Packets
{
    public class HandshakeTokenFactory<T> : TokenFactory<T> where T : HandshakeToken, new()
    {
        private IIDFacade _idFacade;
        private ID _id;
        public HandshakeTokenFactory(IIDFacade idFacade, ID id = null)
        {
            _idFacade = idFacade;
            _id = id;
        }

        public async Task<T> CreateHandshakeTokenAsync(string encodedToken)
        {
            ///////////////////////////////////////////////////////////////////
            //If the token has a signature, then the base will return a locked
            //token, we need to extract the signature from the encoded token
            //supplied to the base. And we'll add the signature in this method
            //instead
            ///////////////////////////////////////////////////////////////////
            string[] splittedEncodedToken = encodedToken.Split('.');

            T t = base.CreateToken(splittedEncodedToken[0] + "." + splittedEncodedToken[1]);

            //decode header to get the ID address and the accessible attribtue
            string jsonDecodedPayload = Base64Decode(splittedEncodedToken[1]);
            Dictionary<string, string> payload = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonDecodedPayload);

            t.Id = await RetrieveID(payload);

            //If there was a signature in the encoded token, we sign it
            ApplySignature(t, splittedEncodedToken);

            return t;
        }

        public T ApplySignature(T t, string[] splittedEncodedToken)
        {
            string signature = null;
            if (splittedEncodedToken.Length == 3)
            {
                signature = splittedEncodedToken[2];
                t.SignAndLock(signature);
            }
            return t;
        }
        /// <summary>
        /// Instantiate the ID object from the address found in the payload.
        /// Will only load the attributes if accessible attribute is specified.
        /// Will only load an id if it was not supplied in the constructor.
        /// </summary>
        /// <returns>The identifier.</returns>
        /// <param name="payload">Payload.</param>
        public async Task<ID> RetrieveID(Dictionary<string, string> payload)
        {
            //get the accessible attributes
            if (payload.ContainsKey("accAttr"))
            {
                //Attempt to load the ID with only the accessible attributes
                string[] accessibleAttributes = JsonConvert.DeserializeObject<string[]>(payload["accAttr"]);

                if (_id == null)
                    _id = await _idFacade.GetIDAsync(payload["idAddr"], accessibleAttributes);
                else
                    _id.Attributes = await _idFacade.GetAttributesAsync(_id, accessibleAttributes);
            }
            else
            {
                //Attempt to load the ID with no attributes
                if (_id == null)
                    _id = await _idFacade.GetIDAsync(payload["idAddr"], new string[] { });
            }


            return _id;
        }
    }
}
