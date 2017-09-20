using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lynx.Core.Facade;
using Lynx.Core.Facade.Interfaces;
using Lynx.Core.Models.IDSubsystem;
using Newtonsoft.Json;

namespace Lynx.Core.Communications.Packets
{
    public class InfoRequestTokenFactory<T> : HandshakeTokenFactory<T> where T : InfoRequestSynAck, new()
	{
		public InfoRequestTokenFactory(IIDFacade idFacade) : base(idFacade)
		{
		}

		public async Task<T> CreateInfoRequestTokenAsync(string encodedToken)
		{
			///////////////////////////////////////////////////////////////////
			//If the token has a signature, then the base will return a locked
			//token, we need to extract the signature from the encoded token
			//supplied to the base. And we'll add the signature in this method
			//instead
			///////////////////////////////////////////////////////////////////
			string[] splittedEncodedToken = encodedToken.Split('.');

			T t = base.CreateToken(splittedEncodedToken[0] + "." + splittedEncodedToken[1]);

			//Decode header to get the ID address and the accessible attribtues
			string jsonDecodedPayload = Base64Decode(splittedEncodedToken[1]);
			Dictionary<string, string> payload = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonDecodedPayload);

            t.Id = await RetrieveID(payload);

			//get the requested attributes
			if (payload.ContainsKey("reqAttr"))
			{
				string[] requestedAttributes = JsonConvert.DeserializeObject<string[]>(payload["reqAttr"]);
				t.RequestedAttributes = requestedAttributes;
			}

			//If there was a signature in the encoded token, we sign it
			ApplySignature(t, splittedEncodedToken);

			return t;
		}
	}
}