using Android.Content;
using MvvmCross.Droid.Platform;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform.Platform;

namespace Lynx.Droid
{
    public class Setup : MvxAndroidSetup
    {
        private AndroidSpecificDataService _dataService;

        public Setup(Context applicationContext) : base(applicationContext)
        {
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
