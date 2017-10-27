using System;
using Plugin.Fingerprint.Dialog;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace Lynx.Droid.Views
{
    public class FingerprintLoginDialogFragment : FingerprintDialogFragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = base.OnCreateView(inflater, container, savedInstanceState);

            var image = view.FindViewById<ImageView>(Plugin.Fingerprint.Resource.Id.fingerprint_imgFingerprint);
            image.SetColorFilter(Android.Graphics.Color.Black);

            return view;
        }
    }
}
