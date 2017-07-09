using Android.App;
using Android.Content.PM;
using MvvmCross.Droid.Views;

namespace Lynx.Droid
{
    [Activity(
        Label = "Lynx.Droid"
        , MainLauncher = true
        , Icon = "@mipmap/icon"
        , Theme = "@style/Lynx.Splash"
        , NoHistory = true
        , ScreenOrientation = ScreenOrientation.Portrait)]
    public class SplashScreen : MvxSplashScreenActivity
    {
        public SplashScreen()
            : base(Resource.Layout.SplashScreen)
        {
        }
    }
}
