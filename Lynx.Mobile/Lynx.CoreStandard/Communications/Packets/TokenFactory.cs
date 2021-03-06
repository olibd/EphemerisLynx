﻿using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using Lynx.Core.Models.IDSubsystem;
using Lynx.Core.Facade;
using System.Threading.Tasks;

namespace Lynx.Core.Communications.Packets
{
    public class TokenFactory<T> where T : Token, new()
    {

        public T CreateToken(string encodedToken)
        {
            //removed the type from the header if it is there
            string untypedEncodedToken = encodedToken.Split(':')[1];
            //split the token in its 3 parts
            string[] splittedEncodedToken = untypedEncodedToken.Split('.');
            string jsonDecodedHeader = Base64Decode(splittedEncodedToken[0]);
            string jsonDecodedPayload = Base64Decode(splittedEncodedToken[1]);

            //define basic sanitation settings
            var settings = new JsonSerializerSettings();
            settings.StringEscapeHandling = StringEscapeHandling.EscapeNonAscii;

            Dictionary<string, string> header = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonDecodedHeader, settings);
            Dictionary<string, string> payload = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonDecodedPayload, settings);

            T t = Activator.CreateInstance(typeof(T), new object[] { header, payload }) as T;

            if (splittedEncodedToken.Length == 3)
            {
                string sig = splittedEncodedToken[2];
                t.SignAndLock(sig);
            }
            return t;
        }

        public string Base64Decode(string encodedText)
        {
            byte[] plainTextBytes = Convert.FromBase64String(encodedText);
            return System.Text.Encoding.UTF8.GetString(plainTextBytes, 0, plainTextBytes.Length);
        }
    }
}
