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
    [Activity(Label = "View for MainViewModel")]
    public class MainView : MvxActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            BindConfirmationInteraction();
            SetContentView(Resource.Layout.MainView);
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
            var confirmationRequest = eventArgs.Value;

            AlertDialog.Builder alert = new AlertDialog.Builder(this);

            alert.SetTitle("Confirmation Needed")
                 .SetMessage(confirmationRequest.Query)
                 .SetPositiveButton("I Confirm!", (senderAlert, e) => confirmationRequest.Callback(true))
                 .SetNegativeButton("I'll check again", (senderAlert, e) => confirmationRequest.Callback(false));

            alert.Show();
        }

        private void BindConfirmationInteraction()
        {
            var set = this.CreateBindingSet<MainView, MainViewModel>();
            set.Bind(this).For(view => view.Interaction).To(viewModel => viewModel.ConfirmationInteraction).OneWay();
            set.Apply();
        }
    }
}
