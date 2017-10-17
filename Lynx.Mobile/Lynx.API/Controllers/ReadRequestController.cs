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
using Attribute = Lynx.Core.Models.IDSubsystem.Attribute;
using Lynx.Core.PeerVerification;
using Lynx.Core.Crypto.Interfaces;
using Lynx.Core.Communications.Packets.Interfaces;
using System.Threading;
using System.Net.Http;
using Lynx.API.Worker.DTO;

namespace Lynx.API.Controllers
{
    [Route("api/[controller]")]
    public class ReadRequestController : Controller
    {
        private ClientContext _clientContext;
        private IServiceProvider _serviceProvider;
        private IMapper<ID> _idMapper;
        private Web3 _web3;

        public ReadRequestController(ClientContext clientContext, IMapper<ID> idMapper, Web3 web3, IServiceProvider serviceProvider)
        {
            _clientContext = clientContext;
            _idMapper = idMapper;
            _web3 = web3;
            _serviceProvider = serviceProvider;
        }

        // POST api/readrequest
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]ReadRequestDTO value)
        {
            //TODO: Temporary seeding of the database, for demonstration purposes
            if (GetClientByApiKey("1234") == null)
            {
                Client newClient = new Client()
                {
                    APIKey = "1234",
                    IDUID = 1,
                    PrivateKey = "9e6a6bf412ce4e3a91a33c7c0f6d94b3127b8d4f5ed336210a672fe595bf1769"
                };

                _clientContext.Add(newClient);
                _clientContext.SaveChanges();
            }

            //Get the client by the supplied API key
            Client client;
            if ((client = GetClientByApiKey(value.APIKey)) == null)
                return Unauthorized();

            //Setup the Info Request Job
            InfoRequestWorker worker = new InfoRequestWorker(_clientContext, _idMapper, _web3, _serviceProvider);
            InfoRequestJobDTO dto = await worker.InitiateJob(client);
            BackgroundJob.Enqueue<InfoRequestWorker>(w => w.CompleteJob(dto, value.CallbackEndpoint));

            //Prepare the SYN for output
            StringToQRSVGValueConverter valConv = new StringToQRSVGValueConverter();
            string qrEncodedSyn = valConv.Convert(dto.EncodedSyn).Content;
            ContentResult content = Content(qrEncodedSyn);
            content.ContentType = "image/svg+xml";

            return content;
        }

        private Client GetClientByApiKey(string apiKey)
        {
            return _clientContext.Clients.SingleOrDefault(c => c.APIKey == apiKey);
        }
    }
}
