using System;
using Android.App;
using Android.OS;
using Lynx.Core.ViewModels;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Droid.Support.V4;

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