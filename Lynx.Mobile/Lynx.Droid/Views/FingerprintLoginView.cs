using System;
using Android.OS;
using Android.App;
using MvvmCross.Droid.Views;
using Plugin.CurrentActivity;
using Plugin.Fingerprint;


namespace Lynx.Droid.Views
{
    [Activity(Label = "View for FingerprintLoginViewModel")]
    public class FingerprintLoginView : MvxActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.FingerprintLoginView);
            CrossFingerprint.SetCurrentActivityResolver(() => CrossCurrentActivity.Current.Activity);
        }
    }
}
