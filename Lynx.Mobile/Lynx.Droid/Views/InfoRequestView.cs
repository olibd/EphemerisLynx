using System;
using Android.App;
using Android.OS;
using Lynx.Core.ViewModels;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Droid.Support.V4;

namespace Lynx.Droid.Views
{
    [Activity(Label = "View for InfoRequestViewModel")]
    public class InfoRequestView : MvxFragmentActivity
    {
        public InfoRequestView()
        {
        }

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.InfoRequestView);
			BindCommand();
		}

		private void BindCommand()
		{
			var set = this.CreateBindingSet<InfoRequestView, InfoRequestViewModel>();
			set.Apply();
		}
    }
}
