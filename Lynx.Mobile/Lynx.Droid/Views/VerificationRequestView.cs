using System;
using Android.App;
using Android.OS;
using Lynx.Core.Models.Interactions;
using Lynx.Core.ViewModels;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Core.ViewModels;
using MvvmCross.Droid.Views;
using MvvmCross.Platform.Core;

namespace Lynx.Droid.Views
{
    [Activity(Label = "View for VerificationRequestViewModel")]
    public class VerificationRequestView : MvxActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.VerificationRequestView);
            BindConfirmationInteraction();
        }

        private IMvxInteraction<BooleanInteraction> _interaction;
        public IMvxInteraction<BooleanInteraction> Interaction
        {
            get => _interaction;
            set
            {
                if (_interaction != null)
                    _interaction.Requested -= OnInteractionRequested;

                _interaction = value;
                _interaction.Requested += OnInteractionRequested;
            }
        }

        private void OnInteractionRequested(object sender, MvxValueEventArgs<BooleanInteraction> eventArgs)
        {
            BooleanInteraction confirmationRequest = eventArgs.Value;

            AlertDialog.Builder alert = new AlertDialog.Builder(this);

            alert.SetTitle("Identity Verifed")
                 .SetMessage(confirmationRequest.Query)
                 .SetPositiveButton("Ok", (senderAlert, e) => confirmationRequest.Callback(true));

            RunOnUiThread(() =>
            {
                alert.Show();
            });
        }

        private void BindConfirmationInteraction()
        {
            var set = this.CreateBindingSet<VerificationRequestView, VerificationRequestViewModel>();
            set.Bind(this).For(view => view.Interaction).To(viewModel => viewModel.ConfirmationInteraction).OneWay();
            set.Apply();
        }
    }
}
