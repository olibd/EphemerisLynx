using System;
using Plugin.Fingerprint.Dialog;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Android.Graphics.Drawables;


namespace Lynx.Droid.Views
{
    public class FingerprintLoginDialogFragment : FingerprintDialogFragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = base.OnCreateView(inflater, container, savedInstanceState);

            var image = view.FindViewById<ImageView>(Plugin.Fingerprint.Resource.Id.fingerprint_imgFingerprint);
            image.SetBackgroundColor(Color.Gray);//change background color of fingerprint image

            return view;
        }
    }
}
