using System;
using Android.App;
using Android.OS;
using Lynx.Core.Interactions;
using Lynx.Core.ViewModels;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Core.ViewModels;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Platform.Core;

namespace Lynx.Droid.Views
{
    [Activity(Label = "View for CertifyViewModel")]
    public class CertifyView : MvxFragmentActivity
    {
        private IMvxInteraction<UserFacingErrorInteraction> _displayErrorInteraction;
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

        public CertifyView()
        {
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.CertifyView);
            BindCommand();
        }

        private void BindCommand()
        {
            var set = this.CreateBindingSet<CertifyView, CertifyViewModel>();
            set.Bind(this)
               .For(view => view.DisplayErrorInteraction)
               .To(viewModel => viewModel.DisplayErrorInteraction)
               .OneWay();
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