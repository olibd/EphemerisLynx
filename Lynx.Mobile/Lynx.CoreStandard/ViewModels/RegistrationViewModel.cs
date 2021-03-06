using System;
using System.Threading.Tasks;
using Lynx.Core.Facade.Interfaces;
using Lynx.Core.Interfaces;
using Lynx.Core.Mappers.IDSubsystem.Strategies;
using Lynx.Core.Models;
using Lynx.Core.Models.IDSubsystem;
using Lynx.Core.Models.Interactions;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using Attribute = Lynx.Core.Models.IDSubsystem.Attribute;

namespace Lynx.Core.ViewModels
{
    public class RegistrationViewModel : MvxViewModel
    {
        private string _serializedID = "0";
        private IIDFacade _idFacade;
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Cell { get; set; }
        public string Address { get; set; }
        public string SerializedID
        {
            get { return _serializedID; }
            set { SetProperty(ref _serializedID, value); }
        }
        public ID ID { get; set; }
        public MvxInteraction<BooleanInteraction> ConfirmationInteraction { get; set; }
        private readonly IMvxNavigationService _navigationService;

        public IMvxCommand DeployIDCommand => new MvxCommand(DeployConfirm, () => SubmitReady);

        private bool _submitReady = true;
        private bool SubmitReady
        {
            get => _submitReady;
            set
            {
                _submitReady = value;
                RaisePropertyChanged(() => DeployIDCommand);
            }
        }


        public RegistrationViewModel(IMvxNavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        //TODO: For more information see: https://www.mvvmcross.com/documentation/fundamentals/navigation
        public void Init()
        {
            ConfirmationInteraction = new MvxInteraction<BooleanInteraction>();

            ID = new ID();
            Mvx.RegisterSingleton(() => ID);
        }

        public override void Start()
        {
            //TODO: Add starting logic here
        }

        /// <summary>
        /// Ask for user confirmation of deployment then deploys.
        /// </summary>
        private void DeployConfirm()
        {
            BooleanInteraction confirmationRequest = new BooleanInteraction
            {
                Callback = async (bool ok) =>
                {
                    if (ok)
                    {
                        SubmitReady = false;
                        await Deploy();
                        await _navigationService.Navigate<IDViewModel>();
                    }
                },

                Query = "Do you confirm that the information supplied is accurate?"
            };

            ConfirmationInteraction.Raise(confirmationRequest);
        }

        private async Task Deploy()
        {
            BuildID();

            await DeployToBlockchain();
            await SaveIDToDB();

        }

        private async Task SaveIDToDB()
        {
            IMapper<ID> idMapper = Mvx.Resolve<IMapper<ID>>();
            await idMapper.SaveAsync(ID);
            SerializedID = "ID UID: " + ID.UID;
            Mvx.Resolve<IPlatformSpecificDataService>().IDUID = ID.UID;
        }

        private async Task DeployToBlockchain()
        {
            _idFacade = Mvx.Resolve<IIDFacade>();
            await _idFacade.DeployAsync(ID);

            Mvx.Resolve<IPlatformSpecificDataService>().IDAddress = ID.Address;

        }

        private void BuildID()
        {
            //create some dummy attributes
            Attribute firstname = new Attribute()
            {
                Description = "firstname",
                Content = new StringContent(Firstname),
                Hash = "hash" + Firstname,
                Location = "Location" + Firstname,
            };

            Attribute lastname = new Attribute()
            {
                Description = "lastname",
                Hash = "hash" + Lastname,
                Location = "Location" + Lastname,
                Content = new StringContent(Lastname)
            };

            Attribute cell = new Attribute()
            {
                Description = "cell",
                Hash = "hash" + Cell,
                Location = "Location" + Cell,
                Content = new StringContent(Cell)
            };

            Attribute address = new Attribute()
            {
                Description = "address",
                Hash = "hash" + Address,
                Location = "Location" + Address,
                Content = new StringContent(Address)
            };

            ID.AddAttribute(firstname);
            ID.AddAttribute(lastname);
            ID.AddAttribute(cell);
            ID.AddAttribute(address);
        }
    }
}