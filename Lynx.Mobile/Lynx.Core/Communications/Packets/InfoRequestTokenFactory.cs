using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lynx.Core.Facade;
using Lynx.Core.Facade.Interfaces;
using Lynx.Core.Models.IDSubsystem;
using Newtonsoft.Json;

namespace Lynx.Core.Communications.Packets
{
    public class InfoRequestTokenFactory<T> : TokenFactory<T> where T : InfoRequestSynAck, new()
	{
		private IIDFacade _idFacade;
		public InfoRequestTokenFactory(IIDFacade idFacade)
		{
			_idFacade = idFacade;
		}

		public async Task<T> CreateInfoRequestTokenAsync(string encodedToken)
		{
			//decode header to get the ID address and the accessible attribtue
			string[] splittedEncodedToken = encodedToken.Split('.');

			///////////////////////////////////////////////////////////////////
			//If the token has a signature, then the base will return a locked
			//token, we need to extract the signature from the encoded token
			//supplied to the base. And we'll add the signature in this method
			//instead
			///////////////////////////////////////////////////////////////////
			string signature = null;
			if (splittedEncodedToken.Length == 3)
			{
				//save a copy of the signature
				signature = splittedEncodedToken[2];
				//remove the signature from the encodedToken
				encodedToken = splittedEncodedToken[0] + "." + splittedEncodedToken[1];
			}

			T t = base.CreateToken(encodedToken);

			///////////////////////////////////////////////////////////////////
			//Instantiate the ID object from the address found in the payload//
			///////////////////////////////////////////////////////////////////
			string jsonDecodedPayload = Base64Decode(splittedEncodedToken[1]);
			Dictionary<string, string> payload = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonDecodedPayload);

			ID id;

			//get the accessible attributes
			if (payload.ContainsKey("accAttr"))
			{
				//Attempt to load the ID with only the accessible attributes
				string[] accessibleAttributes = JsonConvert.DeserializeObject<string[]>(payload["accAttr"]);
				id = await _idFacade.GetIDAsync(payload["idAddr"], accessibleAttributes);
			}
			else
				//Attempt to load the ID without all attributes
				id = await _idFacade.GetIDAsync(payload["idAddr"]);

			t.Id = id;

			//get the requested attributes
			if (payload.ContainsKey("reqAttr"))
			{
				string[] requestedAttributes = JsonConvert.DeserializeObject<string[]>(payload["reqAttr"]);
				t.RequestedAttributes = requestedAttributes;
			}

			//If there was a signature in the encoded token, we sign it
			if (signature != null)
				t.SignAndLock(signature);

			return t;
		}
	}
}