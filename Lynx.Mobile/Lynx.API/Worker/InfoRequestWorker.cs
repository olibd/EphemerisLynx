using System;
using System.Linq;
using System.Threading.Tasks;
using Lynx.API.Models;
using Lynx.Core;
using Lynx.Core.Facade.Interfaces;
using Lynx.Core.Interfaces;
using Lynx.Core.Mappers.IDSubsystem.Strategies;
using Lynx.Core.Models.IDSubsystem;
using Microsoft.Extensions.DependencyInjection;
using Lynx.Core.Facade;
using Nethereum.Web3;
using Lynx.Core.PeerVerification;
using Lynx.Core.Crypto.Interfaces;
using Lynx.Core.Communications.Packets.Interfaces;
using System.Threading;
using System.Net.Http;
using Lynx.API.Worker.DTO;
using Newtonsoft.Json;

namespace Lynx.API
{
    /// <summary>
    /// The info request worker is a class that can be used to initiate a fire
    /// and forget info request task with Hangfire. This is useful when you want
    /// to run a task after an HTTP request is completed.
    /// </summary>
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

        /// <summary>
        /// Initiates and suspends the job.
        /// </summary>
        /// <returns>A DTO containing information about the state of the
        /// suspended job. This DTO is used to complete the job</returns>
        /// <param name="client">Client.</param>
        public async Task<InfoRequestJobDTO> InitiateJob(Client client)
        {
            await Setup(client);
            InfoRequestJobDTO dto = new InfoRequestJobDTO()
            {
                ClientID = client.Id,
                EncodedSyn = _infoRequester.CreateEncodedSyn(),
                SessionID = _infoRequester.SuspendSession()
            };

            return dto;
        }

        /// <summary>
        /// Completes the job. To be call in Hangfire
        /// </summary>
        /// <returns>The job.</returns>
        /// <param name="dto">Suspended Job DTO, used to restore and
        /// complete the job.</param>
        /// <param name="callbackEndpoint">Callback endpoint (where the
        /// result of the job should be sent).</param>
        public async Task CompleteJob(InfoRequestJobDTO dto, string callbackEndpoint)
        {
            await Setup(dto.ClientID);

            ManualResetEvent waitHandle = new ManualResetEvent(false);
            ID id = null;
            _infoRequester.HandshakeComplete += (sender, e) =>
            {
                id = e.ReceivedID;
                waitHandle.Set();
            };
            _infoRequester.ResumeSession(dto.SessionID);

            if (waitHandle.WaitOne(100000))
            {
                await new HttpClient().PostAsync(callbackEndpoint, new System.Net.Http.StringContent(JsonConvert.SerializeObject(id)));
            }
            else
            {
                await new HttpClient().PostAsync(callbackEndpoint, new System.Net.Http.StringContent("error"));
            }
        }
    }
}
