using System;
using Android.Content;
using Android.Preferences;
using Lynx.Core.Interfaces;

namespace Lynx.Droid
{
    internal class AndroidSpecificDataService : IPlatformSpecificDataService
    {
        private Context applicationContext;

        public AndroidSpecificDataService(Context applicationContext)
        {
            this.applicationContext = applicationContext;
        }

        public string IDAddress
        {
            get
            {
                ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(applicationContext);
                return prefs.GetString("IDAddress", "0x0");

            }
            set
            {
                ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(applicationContext);
                prefs.Edit()
                    .PutString("IDAddress", value)
                    .Apply();
            }
        }

        public int IDUID
        {
            get
            {
                ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(applicationContext);
                return prefs.GetInt("IDUDID", -1);
            }
            set
            {
                ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(applicationContext);
                prefs.Edit()
                    .PutInt("IDUDID", value)
                    .Apply();
            }
        }

        public string GetDatabaseFile()
        {
            //TODO: use Environment.GetFolderPath to return a path in the application's Data folder
            return ":memory:";
        }
    }
}