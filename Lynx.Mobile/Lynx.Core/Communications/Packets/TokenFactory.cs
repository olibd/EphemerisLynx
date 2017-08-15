using System;
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
            string[] splittedEncodedToken = encodedToken.Split('.');
            string jsonDecodedHeader = Base64Decode(splittedEncodedToken[0]);
            string jsonDecodedPayload = Base64Decode(splittedEncodedToken[1]);

            jsonDecodedHeader = Uri.EscapeDataString(jsonDecodedHeader);
            jsonDecodedPayload = Uri.EscapeDataString(jsonDecodedPayload);

            Dictionary<string, string> header = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonDecodedHeader);
            Dictionary<string, string> payload = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonDecodedPayload);

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
