using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lynx.Core.Facade;
using Lynx.Core.Facade.Interfaces;
using Lynx.Core.Interfaces;
using Lynx.Core.Mappers.IDSubsystem.Strategies;
using Lynx.Core.Models.IDSubsystem;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using MvvmCross.Core.Navigation;

namespace Lynx.Core.ViewModels
{
    public class MainViewModel : MvxViewModel
    {
        private IMvxNavigationService _navigationService;
        private IMvxCommand _checkAndLoadID => new MvxCommand(CheckAndLoadID);
        public IMvxCommand RegisterClickCommand => new MvxCommand(NavigateRegistration);

        public MainViewModel(IMvxNavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public override void Start()
        {
            base.Start();
            _checkAndLoadID.Execute();
        }

        private async void CheckAndLoadID()
        {
            IPlatformSpecificDataService dataService = Mvx.Resolve<IPlatformSpecificDataService>();
            if (dataService.IDUID != -1)
            {
                //TODO: Load from database, and fallback to loading from blockchain in case of exception
                IIDFacade idFacade = Mvx.Resolve<IIDFacade>();

                //ID id = await idFacade.GetIDAsync(dataService.IDAddress);
                ID id = await Mvx.Resolve<IMapper<ID>>().GetAsync(dataService.IDUID);
                Mvx.RegisterSingleton(() => id);
                await _navigationService.Navigate<IDViewModel>();
            }
        }
        private async void NavigateRegistration()
        {
            //TODO: Generate private key and save into file
            //TODO: Encrypt file, store key in keystore, fingerprint locked
            await _navigationService.Navigate<RegistrationViewModel>();
        }
    }
}