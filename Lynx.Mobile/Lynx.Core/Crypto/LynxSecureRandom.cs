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

        public void GetBytes(byte[] output)
        {
            _secureRandom.NextBytes(output);
        }
    }
}
