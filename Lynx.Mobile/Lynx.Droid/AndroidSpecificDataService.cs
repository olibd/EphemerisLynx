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
        private const string fileName = "keys";
        KeyStoreCryptoService keyStoreCryptoService;

        public AndroidSpecificDataService(Context applicationContext)
        {
            this.applicationContext = applicationContext;
            keyStoreCryptoService = new KeyStoreCryptoService();
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

        public IAccountService LoadAccount()
        {
            string path = GetFileInDataDir(fileName);
            try
            {
                string encryptedKey = File.ReadAllText(path);
                return new AccountService(keyStoreCryptoService.DecryptKey(encryptedKey));
            }
            catch (IOException e)
            {
                return null;
            }
        }

        public void SaveAccount(IAccountService accountService)
        {
            string key = accountService.PrivateKey;
            string encryptedKey = keyStoreCryptoService.EncryptKey(key);

            string path = GetFileInDataDir(fileName);
            File.WriteAllText(path, encryptedKey);
        }
    }
}