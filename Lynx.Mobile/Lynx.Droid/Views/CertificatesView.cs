using System;
using Android.App;
using Android.OS;
using Lynx.Core.ViewModels;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Droid.Support.V4;

namespace Lynx.Droid.Views
{
    [Activity(Label = "View for CertificatesViewModel")]
    public class CertificatesView : MvxFragmentActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.CertificatesView);
            BindCommand();
        }

        private void BindCommand()
        {
            var set = this.CreateBindingSet<CertificatesView, CertificatesViewModel>();
            set.Apply();
        }
    }
}
