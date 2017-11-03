using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MvvmCross.Core.ViewModels;
using MvvmCross.Droid.Views;
using MvvmCross.Platform.Core;
using Lynx.Core.ViewModels;
using MvvmCross.Binding.BindingContext;
using Lynx.Core.Interactions;

namespace Lynx.Droid.Views
{
    [Activity(Label = "View for MainViewModel")]
    class MainView : MvxActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.MainView);
        }
    }
}