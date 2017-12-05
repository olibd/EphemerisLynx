using System;
using Android.App;
using Android.OS;
using Lynx.Core.Models.Interactions;
using Lynx.Core.Interactions;
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
            BindInteractions();
        }

        private IMvxInteraction<UserFacingErrorInteraction> _displayErrorInteraction;
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
        public IMvxInteraction<UserFacingErrorInteraction> DisplayErrorInteraction
        {
            get => _displayErrorInteraction;
            set
            {
                if (_displayErrorInteraction != null)
                    _displayErrorInteraction.Requested -= ShowErrorDialog;

                _displayErrorInteraction = value;
                _displayErrorInteraction.Requested += ShowErrorDialog;
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

        private void BindInteractions()
        {
            var set = this.CreateBindingSet<VerificationRequestView, VerificationRequestViewModel>();
            set.Bind(this).For(view => view.Interaction).To(viewModel => viewModel.ConfirmationInteraction).OneWay();
            set.Bind(this).For(view => view.DisplayErrorInteraction).To(viewModel => viewModel.DisplayErrorInteraction).OneWay();
            set.Apply();
        }

        private void ShowErrorDialog(object sender, MvxValueEventArgs<UserFacingErrorInteraction> e)
        {
            RunOnUiThread(() =>
            {
                AlertDialog.Builder alert = new AlertDialog.Builder(this);
                alert.SetTitle("Error")
                    .SetMessage(e.Value.ErrorMessage)
                    .SetPositiveButton("OK", (o, args) => { })
                    .Show();
            });
        }

    }
}
