using System;
using System.IO;
using Android.Content;
using Android.Preferences;
using Lynx.Core;
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
            return GetFileInDataDir("db.sqlite");
        }

        private string GetFileInDataDir(string file)
        {
           return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/" + file;
        }

        //TODO: Android keystore logic to safely store the private key
        public IAccountService LoadAccount()
        {
            string path = GetFileInDataDir("keys");

            try
            {
                string pkey = File.ReadAllText(path);
                return new AccountService(pkey);
            }
            catch (IOException e)
            {
                return null;
            }

        }

        //TODO: Android keystore logic to safely store the private key
        public void SaveAccount(IAccountService accountService)
        {
            string key = accountService.PrivateKey;
            string path = GetFileInDataDir("keys");
            File.WriteAllText(path, key);
        }
    }
}