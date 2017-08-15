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
        private IIDFacade _iDFacade;
        public HandshakeTokenFactory(IIDFacade iDFacade)
        {
            _iDFacade = iDFacade;
        }

        public async Task<T> CreateHandshakeTokenAsync(string encodedToken)
        {
            T t = base.CreateToken(encodedToken);

            //decode header to get the ID address and the accessible attribtue
            string[] splittedEncodedToken = encodedToken.Split('.');
            string jsonDecodedHeader = Base64Decode(splittedEncodedToken[0]);
            jsonDecodedHeader = Uri.EscapeDataString(jsonDecodedHeader);
            Dictionary<string, string> header = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonDecodedHeader);

            //get the accessible attributes
            string[] accessibleAttributes = JsonConvert.DeserializeObject<string[]>(header["accAttr"]);
            ID id = await _iDFacade.GetIDAsync(header["idAddr"], accessibleAttributes);

            t.Id = id;
            return t;
        }
    }
}
