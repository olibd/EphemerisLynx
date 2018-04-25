using Nethereum.Web3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lynx.Core.Interfaces;

namespace Lynx.Core.Facade
{
    public class Facade
    {
        protected Web3 Web3 { get; }
        protected IAccountService AccountService { get; }

        public Facade(Web3 web3, IAccountService accountService)
        {
            Web3 = web3;
            AccountService = accountService;
        }
    }
}
