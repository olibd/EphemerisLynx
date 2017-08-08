using System;
using System.Collections.Generic;
using Lynx.Core.Services;
using Newtonsoft.Json;
using NUnit.Framework;

namespace CoreUnitTests.PCL
{
    public abstract class TokenTest
    {
        protected Token _token;
        protected Dictionary<string, string> _header;
        protected Dictionary<string, string> _payload;

        public virtual void Setup()
        {
            _header = new Dictionary<string, string>();
            _payload = new Dictionary<string, string>();
        }

        [Test]
        public virtual void GetEncodedTokenTest()
        {
            var encodedToken = _token.GetEncodedToken();

            string[] splittedEncodedToken = encodedToken.Split('.');
            string jsonDecodedHeader = Base64Decode(splittedEncodedToken[0]);
            string jsonDecodedPayload = Base64Decode(splittedEncodedToken[1]);

            if (splittedEncodedToken.Length == 3)
            {
                string sig = splittedEncodedToken[2];
                Assert.AreEqual(_token.Signature, sig);
            }


            Dictionary<string, string> decodedHeader = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonDecodedHeader);
            Dictionary<string, string> decodedPayload = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonDecodedPayload);

            AssertDictionariesAreEqual(_header, decodedHeader);
            AssertDictionariesAreEqual(_payload, decodedPayload);
        }

        [Test]
        public virtual void GetUnsignedEncodedTokenTest()
        {
            var encodedToken = _token.GetUnsignedEncodedToken();

            string[] splittedEncodedToken = encodedToken.Split('.');

            if (splittedEncodedToken.Length == 3)
            {
                Assert.Fail();
            }

            string jsonDecodedHeader = Base64Decode(splittedEncodedToken[0]);
            string jsonDecodedPayload = Base64Decode(splittedEncodedToken[1]);

            Dictionary<string, string> decodedHeader = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonDecodedHeader);
            Dictionary<string, string> decodedPayload = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonDecodedPayload);

            AssertDictionariesAreEqual(_header, decodedHeader);
            AssertDictionariesAreEqual(_payload, decodedPayload);
        }

        [Test]
        public virtual void GetEncodedHeaderTest()
        {
            string jsonDecodedHeader = Base64Decode(_token.GetEncodedHeader());

            Dictionary<string, string> decodedHeader = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonDecodedHeader);
            AssertDictionariesAreEqual(_header, decodedHeader);
        }

        [Test]
        public virtual void GetEncodedPayloadTest()
        {
            string jsonDecodedPayload = Base64Decode(_token.GetEncodedPayload());

            Dictionary<string, string> decodePayload = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonDecodedPayload);
            AssertDictionariesAreEqual(_payload, decodePayload);
        }

        [Test]
        public void GetSetOnHeaderTest()
        {
            _token.SetOnHeader("test", "allo");
            Assert.AreEqual("allo", _token.GetFromHeader("test"));
        }

        [Test]
        public void GetSetOnPayloadTest()
        {
            _token.SetOnPayload("test", "allo");
            Assert.AreEqual("allo", _token.GetFromPayload("test"));
        }

        [Test]
        public void SignatureLockTest()
        {
            Assert.False(_token.Locked);

            ////
            //The token is unlocked, let's edit it
            ////
            GetSetOnHeaderTest();
            GetSetOnPayloadTest();

            _token.SignAndLock("sig");
            Assert.True(_token.Locked);

            ////
            //Now that the token is locked, let's try to edit it
            ////
            bool setOnHeaderException = false;
            try
            {
                GetSetOnHeaderTest();
            }
            catch (Exception e)
            {
                setOnHeaderException = true;
            }
            Assert.True(setOnHeaderException);

            bool setOnPayloadException = false;
            try
            {
                GetSetOnPayloadTest();
            }
            catch (Exception e)
            {
                setOnPayloadException = true;
            }
            Assert.True(setOnPayloadException);
        }

        private string Base64Decode(string encodedText)
        {
            byte[] plainTextBytes = Convert.FromBase64String(encodedText);
            return System.Text.Encoding.UTF8.GetString(plainTextBytes, 0, plainTextBytes.Length);
        }

        private void AssertDictionariesAreEqual(Dictionary<string, string> expected, Dictionary<string, string> actual)
        {
            Assert.AreEqual(expected.Count, actual.Count);
            foreach (string key in expected.Keys)
            {
                Assert.AreEqual(expected[key], actual[key]);
            }
        }
    }
}
