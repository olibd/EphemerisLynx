using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.Content;
using Lynx.Core.Networking;
using MvvmCross.Droid.Views;
using MvvmCross.Platform;
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
            SetupPollingCallback();
        }

        private void SetupPollingCallback()
        {
            NetworkRessourcesPollingService NetPS = Mvx.Resolve<NetworkRessourcesPollingService>();
            NetPS.PollingFailure += (sender, e) =>
            {
                Intent it = new Intent("NET_POLL_SNACKBAR");
                it.PutExtra("EXTRA_RETURN_MESSAGE", "Network Connection Lost");
                it.PutExtra("EXTRA_STATUS", "POLL FAILED");
                LocalBroadcastManager.GetInstance(this).SendBroadcast(it);
            };

            NetPS.PollingSuccess += (sender, e) =>
            {
                Intent it = new Intent("NET_POLL_SNACKBAR");
                it.PutExtra("EXTRA_STATUS", "POLL SUCCESS");
                LocalBroadcastManager.GetInstance(this).SendBroadcast(it); ;
            };
        }
    }
}