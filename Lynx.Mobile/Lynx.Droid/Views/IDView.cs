using System;
using Android.App;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Widget;
using Lynx.Core.Models.Interactions;
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

        private ZXingScannerFragment _scanner;

        public IMvxCommand QrCodeScanCommand { get; set; }

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
                    try
                    {
                        Toast.MakeText(this, "Request Scanned. Please wait.", ToastLength.Long).Show();
                    }
                    catch (Exception e)
                    {
                        var f = e;
                    }
                });

                QrCodeScanCommand.Execute(result.Text);
            },
            MobileBarcodeScanningOptions.Default);
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

