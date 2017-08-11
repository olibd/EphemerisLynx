using Android.App;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Widget;
using Lynx.Core.Models.Interactions;
using Lynx.Core.ViewModels;
using Lynx.Droid.Views.Callbacks;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Core.ViewModels;
using MvvmCross.Droid.Views;
using MvvmCross.Platform.Core;
using static Android.Support.Design.Widget.BottomSheetBehavior;

namespace Lynx.Droid.Views
{
    [Activity(Label = "View for IDViewModel")]
    public class IDView : MvxActivity
    {
        private BottomSheetBehavior _bottomSheetBehavior;
        private LinearLayout _bottomSheet;
        private CoordinatorLayout _IDViewLayout;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.IDView);
            _bottomSheet = FindViewById<LinearLayout>(Resource.Id.bottom_sheet);
            _bottomSheetBehavior = BottomSheetBehavior.From(_bottomSheet);
            _IDViewLayout = FindViewById<CoordinatorLayout>(Resource.Id.IDViewLayout);
            _bottomSheetBehavior.SetBottomSheetCallback(new BottomSheetInvalidateParentCallback());
        }
    }
}
