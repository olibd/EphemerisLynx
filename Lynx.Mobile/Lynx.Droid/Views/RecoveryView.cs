using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MvvmCross.Core.ViewModels;
using MvvmCross.Droid.Views;
using MvvmCross.Platform.Core;
using Lynx.Core.ViewModels;
using MvvmCross.Binding.BindingContext;
using Lynx.Core.Interactions;

namespace Lynx.Droid.Views
{
    [Activity(Label = "View for RecoveryViewModel")]
    class RecoveryView : MvxActivity
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

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            BindInteraction();
            SetContentView(Resource.Layout.RecoveryView);
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

        private void BindInteraction()
        {
            var set = this.CreateBindingSet<RecoveryView, RecoveryViewModel>();
            set.Bind(this)
               .For(view => view.DisplayErrorInteraction)
               .To(viewModel => viewModel.DisplayErrorInteraction)
               .OneWay();
            set.Apply();
        }
    }
}