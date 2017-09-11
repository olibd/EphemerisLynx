using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lynx.Core.Facade;
using Lynx.Core.Facade.Interfaces;
using Lynx.Core.Interfaces;
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
        public MainViewModel(IMvxNavigationService navigationService)
        {
            _navigationService = navigationService;
        }
        public override void Appeared()
        {
            base.Appeared();
            _checkAndLoadID.Execute();
        }
        public IMvxCommand RegisterClickCommand => new MvxCommand(NavigateRegistration);
        private async void CheckAndLoadID()
        {
            if (Mvx.Resolve<IPlatformSpecificDataService>().IDUID != -1)
            {
                //TODO: Load from database, and fallback to loading from blockchain in case of exception
                IIDFacade idFacade = Mvx.Resolve<IIDFacade>();
                IPlatformSpecificDataService dataService = Mvx.Resolve<IPlatformSpecificDataService>();

                ID id = Mvx.Resolve<ID>();
                id = await idFacade.GetIDAsync(dataService.IDAddress);
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