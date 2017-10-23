using System;
using MvvmCross.Core.ViewModels;
using MvvmCross.Core.Navigation;
using Lynx.Core.PeerVerification.Interfaces;
using Plugin.Fingerprint;

namespace Lynx.Core.ViewModels
{
    public class FingerprintLoginViewModel : MvxViewModel<IReceiver>
    {

        private IMvxNavigationService _navigationService;
        private string _fingerprintAuthenticationMessage;

        public string FingerprintAuthenticationMessage
        {
            get => _fingerprintAuthenticationMessage;
        }

        public FingerprintLoginViewModel(IMvxNavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public override void Prepare(IReceiver parameter)
        {
            throw new NotImplementedException();
        }

        public IMvxCommand FingerprintLoginCommand => new MvxCommand(FingerprintLoginAsync);

        private async void FingerprintLoginAsync()
        {
            var result = await CrossFingerprint.Current.IsAvailableAsync(true);
            if (result)
            {
                var auth = await CrossFingerprint.Current.AuthenticateAsync("Scan fingerprint to access your ID");
                if (auth.Authenticated)
                {
                    await _navigationService.Navigate<IDViewModel>();
                }
                else{
                    _fingerprintAuthenticationMessage = "Authentication Failed!";
                    RaisePropertyChanged(() => FingerprintAuthenticationMessage);
                }
            }
        }
    }
}
