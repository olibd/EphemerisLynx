using System;
using Android.App;
using Android.Content;
using Android.Support.Design.Widget;
using Android.Views;

namespace Lynx.Droid.Views
{
    public class SnackbarBroadcastReceiver : BroadcastReceiver
    {
        private View _view;
        private Snackbar _snackbar;
        public SnackbarBroadcastReceiver(View view)
        {
            _view = view;
        }

        public override void OnReceive(Context context, Intent intent)
        {
            if (_snackbar == null && intent.GetBooleanExtra("EXTRA_POLL_SUCCESS", false))
            {
                _snackbar = Snackbar.Make(_view, intent.GetStringExtra("EXTRA_RETURN_MESSAGE"), Snackbar.LengthIndefinite);
                _snackbar.SetAction("Dismiss", (obj) => _snackbar.Dismiss());
                _snackbar.Show();
            }
            else
            {
                _snackbar.Dismiss();
                _snackbar = null;
            }
        }
    }
}