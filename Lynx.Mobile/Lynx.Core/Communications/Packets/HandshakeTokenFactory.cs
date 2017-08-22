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
            T t = base.CreateToken(encodedToken);

            //decode header to get the ID address and the accessible attribtue
            string[] splittedEncodedToken = encodedToken.Split('.');
            string jsonDecodedPayload = Base64Decode(splittedEncodedToken[1]);
            Dictionary<string, string> payload = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonDecodedPayload);

            ID id;
            //get the accessible attributes
            if (payload.ContainsKey("accAttr"))
            {
                string[] accessibleAttributes = JsonConvert.DeserializeObject<string[]>(payload["accAttr"]);
                id = await _idFacade.GetIDAsync(payload["idAddr"], accessibleAttributes);
            }
            else
                id = await _idFacade.GetIDAsync(payload["idAddr"]);

            t.Id = id;
            return t;
        }
    }
}
