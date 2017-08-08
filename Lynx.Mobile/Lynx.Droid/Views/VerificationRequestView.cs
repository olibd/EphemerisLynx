using System;
using Android.App;
using Android.OS;
using MvvmCross.Droid.Views;

namespace Lynx.Droid.Views
{
    [Activity(Label = "View for VerificationRequestViewModel")]
    public class VerificationRequestView : MvxActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.VerificationRequestView);
        }
    }
}
