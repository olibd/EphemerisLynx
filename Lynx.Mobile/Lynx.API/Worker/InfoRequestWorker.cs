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
using Hangfire.Server;
using Hangfire.Console;

namespace Lynx.API
{
    public class InfoRequestWorker
    {
        private ClientContext _clientContext;
        private IIDFacade _idFacade;
        private IAttributeFacade _attrFacade;
        private ICertificateFacade _certFacade;
        private IServiceProvider _serviceProvider;
        private ID _id;
        private IMapper<ID> _idMapper;
        private IAccountService _accountService;
        private Web3 _web3;
        private InfoRequester _infoRequester;
        private string[] _reqAttr;

        public InfoRequestWorker(ClientContext clientContext, IMapper<ID> idMapper, Web3 web3, IServiceProvider serviceProvider)
        {
            _clientContext = clientContext;
            _idMapper = idMapper;
            _web3 = web3;
            _serviceProvider = serviceProvider;
            _reqAttr = new string[] { "firstname", "lastname", "cell", "address" };
        }

        private async Task Setup(string clientID)
        {
            Client client = LoadClient(clientID);
            await Setup(client);
        }

        private async Task Setup(Client client)
        {
            await LoadDependencies(client);

            ID id = await LoadID(client.IDUID);

            _infoRequester = LoadInfoRequester(id);
        }

        private InfoRequester LoadInfoRequester(ID id)
        {
            return new InfoRequester(_reqAttr,
                    _serviceProvider.GetService<ITokenCryptoService<IToken>>(),
                    _accountService, id, _idFacade, _attrFacade, _certFacade);
        }

        private Client LoadClient(string clientID)
        {
            return _clientContext.Clients.SingleOrDefault(c => c.Id == clientID);
        }

        private async Task<ID> LoadID(int IDUID)
        {
            return await _idMapper.GetAsync(IDUID);
        }

        private async Task LoadDependencies(Client client)
        {
            _accountService = new AccountService(client.PrivateKey);
            _id = await _idMapper.GetAsync(client.IDUID);
            IContentService contServ = _serviceProvider.GetService<IContentService>();
            _certFacade = new CertificateFacade(_web3, contServ, _accountService);
            _attrFacade = new AttributeFacade(_web3, _certFacade, contServ, _accountService);
            _idFacade = new IDFacade(StaticRessources.FactoryContractAddress, _web3, _attrFacade, _accountService);
        }

        public async Task<InfoRequestSessionDTO> InitiateJob(Client client)
        {
            await Setup(client);
            InfoRequestSessionDTO dto = new InfoRequestSessionDTO()
            {
                ClientID = client.Id,
                EncodedSyn = _infoRequester.CreateEncodedSyn(),
                SessionID = _infoRequester.SuspendSession()
            };

            return dto;
        }

        public async Task CompleteJob(InfoRequestSessionDTO dto, string callbackEndpoint, PerformContext context)
        {
            await Setup(dto.ClientID);

            try
            {
                _infoRequester.ResumeSession(dto.SessionID);

                ManualResetEvent waitHandle = new ManualResetEvent(false);
                ID id = null;
                _infoRequester.handshakeComplete += (sender, e) =>
                {
                    id = e.Id;
                    waitHandle.Set();
                };

                context.WriteLine("Job Resumed");

                if (waitHandle.WaitOne(100000))
                {
                    await new HttpClient().PostAsync(callbackEndpoint, new System.Net.Http.StringContent(id.Address));
                    context.WriteLine("Job Completed");
                }
                else
                {
                    await new HttpClient().PostAsync(callbackEndpoint, new System.Net.Http.StringContent("error"));
                    context.WriteLine("Job Failed");
                }
            }
            catch (Exception e)
            {
                await new HttpClient().PostAsync(callbackEndpoint, new System.Net.Http.StringContent(e.Message + "--------" + e.StackTrace));
            }

        }
    }
}
