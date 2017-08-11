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

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.IDView);
            _bottomSheet = FindViewById<LinearLayout>(Resource.Id.bottom_sheet);
            _bottomSheetBehavior = BottomSheetBehavior.From(_bottomSheet);
            _IDViewLayout = FindViewById<CoordinatorLayout>(Resource.Id.IDViewLayout);
            _bottomSheetBehavior.SetBottomSheetCallback(new BottomSheetInvalidateParentCallback());

            _scanner = new ZXingScannerFragment();
            _scanner.ScanningOptions = MobileBarcodeScanningOptions.Default;
            SupportFragmentManager.BeginTransaction()
                .Add(Resource.Id.IDViewLayout, _scanner, "ZXINGSCANNER")
                .Commit();
            
        }

        protected override void OnResume()
        {
            base.OnResume();
            if (PermissionsHandler.NeedsPermissionRequest(this))
            {
                PermissionsHandler.RequestPermissionsAsync(this);
            }
            Handler handler = new Handler(Looper.MainLooper);

            _scanner.StartScanning(result =>
            {
                handler.Post(() =>
                {
                    Toast toast = new Toast(this);
                    toast.Duration = ToastLength.Short;
                    toast.SetText(result.Text);
                    toast.Show();
                });
            }, MobileBarcodeScanningOptions.Default);
        }
    }
}

