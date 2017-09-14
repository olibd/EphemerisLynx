using System;
using Android.App;
using Android.OS;
using Android.Support.Design.Widget;
using Lynx.Core.ViewModels;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Droid.Support.V4;
using ZXing.Net.Mobile.Android;

namespace Lynx.Droid.Views
{
    [Activity(Label = "View for CertifyViewModel")]
    public class CertifyView : MvxFragmentActivity
    {
        public CertifyView()
        {
        }

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.CertifyView);
			BindCommand();
		}

		private void BindCommand()
		{
			var set = this.CreateBindingSet<CertifyView, CertifyViewModel>();
            set.Apply();
		}
    }
}
