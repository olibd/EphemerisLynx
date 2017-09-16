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
        public HandshakeTokenFactory(IIDFacade idFacade)
        {
            _idFacade = idFacade;
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

        public async Task<ID> RetrieveID(Dictionary<string, string> payload)
        {
            //Instantiate the ID object from the address found in the payload
            ID id;

            //get the accessible attributes
            if (payload.ContainsKey("accAttr"))
            {
                //Attempt to load the ID with only the accessible attributes
                string[] accessibleAttributes = JsonConvert.DeserializeObject<string[]>(payload["accAttr"]);
                id = await _idFacade.GetIDAsync(payload["idAddr"], accessibleAttributes);
            }
            else
                //Attempt to load the ID with all attributes
                id = await _idFacade.GetIDAsync(payload["idAddr"]);

            return id;
        }
    }
}
