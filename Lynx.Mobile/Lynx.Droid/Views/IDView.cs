using System;
using Android.App;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Widget;
using Lynx.Core.Models.Interactions;
using Lynx.Core.Interactions;
using Lynx.Core.ViewModels;
using Lynx.Droid.Views.Callbacks;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Core.ViewModels;
using MvvmCross.Droid.Views;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Platform.Core;
using ZXing;
using ZXing.Mobile;
using ZXing.Net.Mobile.Android;
using static Android.Support.Design.Widget.BottomSheetBehavior;

namespace Lynx.Droid.Views
{
    [Activity(Label = "View for IDViewModel")]
    public class IDView : MvxFragmentActivity
    {
        private LinearLayout _bottomSheet;
        private IMvxInteraction<UserFacingErrorInteraction> _displayError;

        private ZXingScannerFragment _scanner;

        public IMvxCommand QrCodeScanCommand { get; set; }

        public IMvxInteraction<UserFacingErrorInteraction> DisplayError
        {
            get => _displayError;
            set
            {
                if (_displayError != null)
                    _displayError.Requested -= ShowErrorDialog;

                _displayError = value;
                _displayError.Requested += ShowErrorDialog;
            }
        }


        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.IDView);
            _bottomSheet = FindViewById<LinearLayout>(Resource.Id.bottom_sheet);

            _scanner = new ZXingScannerFragment
            {
                ScanningOptions = MobileBarcodeScanningOptions.Default
            };
            SupportFragmentManager.BeginTransaction()
                .Add(Resource.Id.ZXingScannerLayout, _scanner, "ZXINGSCANNER")
                .Commit();

            BindCommands();
        }

        protected override void OnResume()
        {
            base.OnResume();
            if (PermissionsHandler.NeedsPermissionRequest(this))
            {
                PermissionsHandler.RequestPermissionsAsync(this);
            }
            _scanner.StartScanning(result =>
            {
                if (!result.Text.Contains(":"))
                    return;

                RunOnUiThread(() =>
                {
                    Toast.MakeText(this, "Request Scanned. Please wait.", ToastLength.Long).Show();
                });

                QrCodeScanCommand.Execute(result.Text);
            },
            MobileBarcodeScanningOptions.Default);
        }

        private void ShowErrorDialog(object sender, MvxValueEventArgs<UserFacingErrorInteraction> e)
        {
            RunOnUiThread(() =>
            {
                AlertDialog.Builder alert = new AlertDialog.Builder(this);
                alert.SetTitle("Error")
                    .SetMessage(e.Value.Exception.Message)
                    .SetPositiveButton("OK", (o, args) => { })
                    .Show();
            });
        }

        private void BindCommands()
        {
            var set = this.CreateBindingSet<IDView, IDViewModel>();
            set.Bind(this)
                .For(view => view.QrCodeScanCommand)
                .To(viewModel => viewModel.QrCodeScanCommand)
                .OneWay()
                .Apply();
        }

    }
}

