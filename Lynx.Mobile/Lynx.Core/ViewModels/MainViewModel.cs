using System;
using System.Threading.Tasks;
using Lynx.Core.Facade.Interfaces;
using Lynx.Core.Mappers.IDSubsystem.Strategies;
using Lynx.Core.Models;
using Lynx.Core.Models.IDSubsystem;
using Lynx.Core.Models.Interactions;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using Attribute = Lynx.Core.Models.IDSubsystem.Attribute;

namespace Lynx.Core.ViewModels
{
    public class MainViewModel : MvxViewModel
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

        public MainViewModel()
        {
        }

        //TODO: For more information see: https://www.mvvmcross.com/documentation/fundamentals/navigation
        public void Init()
        {
            ConfirmationInteraction = new MvxInteraction<BooleanInteraction>();

            ID = Mvx.Resolve<ID>();
        }

        public override void Start()
        {
            //TODO: Add starting logic here
        }

        public IMvxCommand DeployIDCommand => new MvxCommand(DeployConfirm);

        private void DeployConfirm()
        {
            BooleanInteraction confirmationRequest = new BooleanInteraction
            {
                Callback = async (bool ok) =>
                {
                    if (ok)
                    {
                        await Deploy();
                    }
                },

                Query = "Do you confirm that the information supplied is accurate?"
            };

            ConfirmationInteraction.Raise(confirmationRequest);
        }

        private Task Deploy()
        {
            BuildID();
            return DeployToBlockchain().ContinueWith((prevtask) =>
            {
                //TODO: add extra logic if the task faulted
                if (!prevtask.IsFaulted)
                    SaveIDToDB();
            });
        }

        private void SaveIDToDB()
        {
            IMapper<ID> idMapper = Mvx.Resolve<IMapper<ID>>();
            idMapper.Save(ID);
            SerializedID = "ID UID: " + ID.UID;
        }

        private Task DeployToBlockchain()
        {
            _idFacade = Mvx.Resolve<IIDFacade>();
            return _idFacade.DeployAsync(ID);
        }

        private void BuildID()
        {
            //create some dummy attributes
            Attribute firstname = new Attribute()
            {
                Content = new StringContent(Firstname)
            };

            Attribute lastname = new Attribute()
            {
                Content = new StringContent(Lastname)
            };

            Attribute cell = new Attribute()
            {
                Content = new StringContent(Cell)
            };

            Attribute address = new Attribute()
            {
                Content = new StringContent(Address)
            };

            ID.AddAttribute("Firstname", firstname);
            ID.AddAttribute("Lastname", lastname);
            ID.AddAttribute("Cell", cell);
            ID.AddAttribute("Address", address);
        }
    }
}