using Android.Content;
using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Crashes;
using MvvmCross.Droid.Platform;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform.Platform;
using Microsoft.Azure.Mobile.Analytics;

namespace Lynx.Droid
{
    public class Setup : MvxAndroidSetup
    {
        private AndroidSpecificDataService _dataService;

        public Setup(Context applicationContext) : base(applicationContext)
        {
            MobileCenter.Start("5bad6674-51e9-4673-9507-a9fdad2e6a27", typeof(Analytics), typeof(Crashes));
            _dataService = new AndroidSpecificDataService(applicationContext);
        }

        protected override IMvxApplication CreateApp()
        {
            return new Core.App(_dataService);
        }

        protected override IMvxTrace CreateDebugTrace()
        {
            return new DebugTrace();
        }
    }
}
