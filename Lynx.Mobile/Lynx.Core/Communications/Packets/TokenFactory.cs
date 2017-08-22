﻿using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using Lynx.Core.Models.IDSubsystem;
using Lynx.Core.Facade;
using Lynx.Core.Facade.Interfaces;
using System.Threading.Tasks;

namespace Lynx.Core.Communications.Packets       
{
    public class TokenFactory<T> where T : Token, new()
    {
        private IIDFacade _iDFacade;

        public TokenFactory(IIDFacade iDFacade)
        {
            _iDFacade = iDFacade; 
        }

		public async Task<T> CreateToken(string encodedToken, string[] accessibleAttributes = null)
        {
            T t;
			var settings = new JsonSerializerSettings();
            settings.StringEscapeHandling = StringEscapeHandling.EscapeNonAscii; 

			string[] splittedEncodedToken = encodedToken.Split('.');
			string jsonDecodedHeader = Base64Decode(splittedEncodedToken[0]);
			string jsonDecodedPayload = Base64Decode(splittedEncodedToken[1]);

			Dictionary<string, string> header = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonDecodedHeader, settings);
			Dictionary<string, string> payload = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonDecodedPayload, settings);

            ID id = await _iDFacade.GetIDAsync(header["idAddr"], accessibleAttributes);

			if (splittedEncodedToken.Length == 3)
			{
				string sig = splittedEncodedToken[2];
				t = Activator.CreateInstance(typeof(T),
				  new object[] { header, payload, id, sig}) as T;
			}
            else
            {
				t = Activator.CreateInstance(typeof(T),
				  new object[] { header, payload, id}) as T;
            }
            return t;
        }

		private string Base64Decode(string encodedText)
		{
			byte[] plainTextBytes = Convert.FromBase64String(encodedText);
			return System.Text.Encoding.UTF8.GetString(plainTextBytes, 0, plainTextBytes.Length);
		}
    }
}
