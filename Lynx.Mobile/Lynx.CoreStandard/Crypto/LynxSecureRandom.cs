using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBitcoin;
using Org.BouncyCastle.Security;

namespace Lynx.Core.Crypto
{
    class LynxSecureRandom : IRandom
    {
        private SecureRandom _secureRandom;

        public LynxSecureRandom()
        {
            //TODO: Seed this properly
            _secureRandom = new SecureRandom();
        }

        /// <summary>
        /// Seeds the array with random bytes.
        /// </summary>
        /// <param name="output">Output.</param>
        public void GetBytes(byte[] output)
        {
            _secureRandom.NextBytes(output);
        }
    }
}
