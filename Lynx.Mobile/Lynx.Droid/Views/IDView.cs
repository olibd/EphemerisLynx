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
        private BottomSheetBehavior _bottomSheetBehavior;
        private LinearLayout _bottomSheet;
        private CoordinatorLayout _IDViewLayout;

        private ZXingScannerFragment _scanner;
        private IMvxCommand _qrCodeScanCommand;

        public IMvxCommand QrCodeScanCommand { get; set; }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.IDView);
            _bottomSheet = FindViewById<LinearLayout>(Resource.Id.bottom_sheet);
            _bottomSheetBehavior = BottomSheetBehavior.From(_bottomSheet);
            _IDViewLayout = FindViewById<CoordinatorLayout>(Resource.Id.IDViewLayout);
            _bottomSheetBehavior.SetBottomSheetCallback(new BottomSheetInvalidateParentCallback());

            _scanner = new ZXingScannerFragment
            {
                ScanningOptions = MobileBarcodeScanningOptions.Default
            };
            SupportFragmentManager.BeginTransaction()
                .Add(Resource.Id.ZXingScannerLayout, _scanner, "ZXINGSCANNER")
                .Commit();

            BindCommand();
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
                QrCodeScanCommand.Execute(result.Text);
            }, 
            MobileBarcodeScanningOptions.Default);
        }

        private void BindCommand()
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

