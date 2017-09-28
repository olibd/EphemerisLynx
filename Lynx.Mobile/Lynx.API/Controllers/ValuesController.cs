using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Lynx.API.Models;
using Lynx.API.ValueConverters;
using Lynx.Core;
using Lynx.Core.Facade.Interfaces;
using Lynx.Core.Interfaces;
using Lynx.Core.Mappers.IDSubsystem.Strategies;
using Lynx.Core.Models.IDSubsystem;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.DependencyInjection;
using Lynx.Core.Facade;
using Nethereum.Web3;

namespace Lynx.API.Controllers
{
    [Route("api/[controller]")]
    public class ReadRequestController : Controller
    {
        private ClientContext _clientContext;
        private IServiceProvider _serviceProvider;
        private IIDFacade _idFacade;
        private ID _id;
        private IMapper<ID> _idMapper;
        private IAccountService _accountService;
        private Web3 _web3;


        public ReadRequestController(ClientContext clientContext, IServiceProvider serviceProvider)
        {
            _clientContext = clientContext;
            _serviceProvider = serviceProvider;
            _idMapper = _serviceProvider.GetService<IMapper<ID>>();
            _web3 = _serviceProvider.GetService<Web3>();
        }

        // GET api/readrequest
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/readrequest/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/readrequest
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]ReadRequestDTO value)
        {
            //TODO: TEMPORARY, FOR TESTING PURPOSES
            Client newClient = new Client()
            {
                APIKey = "1234",
                IDUID = 1
            };

            _clientContext.Add(newClient);
            _clientContext.SaveChanges();

            Client client;
            if ((client = GetClientByApiKey(value.APIKey)) == null)
                return Unauthorized();

            await Setup(client);

            Session session = new Session()
            {
                CallbackEndpoint = value.CallbackEndpoint,
                Client = client
            };

            client.Sessions.Add(session);
            _clientContext.SaveChanges();

            string syn = "test syn";

            //BackgroundJob.Enqueue(() => );


            StringToSVGValueConverter valConv = new StringToSVGValueConverter();
            string qrEncodedSyn = valConv.Convert(syn).Content;

            ContentResult content = Content(qrEncodedSyn);
            content.ContentType = "image/svg+xml";

            return content;
        }

        // PUT api/readrequest/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/readrequest/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        private Client GetClientByApiKey(string apiKey)
        {
            return _clientContext.Clients.SingleOrDefault(c => c.APIKey == apiKey);
        }

        private async Task Setup(Client client)
        {
            _accountService = new AccountService(client.PrivateKey);
            _id = await _idMapper.GetAsync(client.IDUID);
            IContentService contServ = _serviceProvider.GetService<IContentService>();
            ICertificateFacade certFacade = new CertificateFacade(_web3, contServ, _accountService);
            IAttributeFacade attrFacade = new AttributeFacade(certFacade, contServ, _accountService);
            _idFacade = new IDFacade(null, _web3, attrFacade, _accountService);
        }
    }
}
