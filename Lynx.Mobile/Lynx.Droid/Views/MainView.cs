using System;
using Android.App;
using Android.OS;
using MvvmCross.Droid.Views;
using Plugin.CurrentActivity;
using Plugin.Fingerprint;

namespace Lynx.Droid.Views
{
    [Activity(Label = "View for MainViewModel")]
    class MainView : MvxActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.MainView);
            CrossFingerprint.SetCurrentActivityResolver(() => CrossCurrentActivity.Current.Activity);
            CrossFingerprint.SetDialogFragmentType<FingerprintLoginDialogFragment>();
        }

    }
}