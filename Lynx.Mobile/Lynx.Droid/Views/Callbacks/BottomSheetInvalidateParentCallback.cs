using System;
using Android.Views;
using static Android.Support.Design.Widget.BottomSheetBehavior;
namespace Lynx.Droid.Views.Callbacks
{
    public class BottomSheetInvalidateParentCallback : BottomSheetCallback
    {
        public override void OnSlide(View bottomSheet, float slideOffset)
        {
            ((View)bottomSheet.Parent).Invalidate();
        }

        /// <summary>
        /// UNIMPLEMENTED DO NOT CALL.
        /// </summary>
        /// <param name="bottomSheet">Bottom sheet.</param>
        /// <param name="newState">New state.</param>
        public override void OnStateChanged(View bottomSheet, int newState)
        {
            return;
        }
    }
}
