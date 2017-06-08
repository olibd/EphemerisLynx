using Nethereum.Web3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lynx.Core.Facade
{
    class Facade
    {
        protected Web3 _web3;
        protected string _address;
        protected string _password;

        public Facade(string address, string password, Web3 web3)
        {
            _web3 = web3;
            _address = address;
            _password = password;
        }
    }
}
