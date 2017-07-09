using Nethereum.Web3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lynx.Core.Facade
{
    public class Facade
    {
        protected Web3 Web3 { get; }
        protected string Address { get; }
        protected string Password { get; }

        public Facade(string address, string password, Web3 web3)
        {
            Web3 = web3;
            Address = address;
            Password = password;
        }
    }
}
