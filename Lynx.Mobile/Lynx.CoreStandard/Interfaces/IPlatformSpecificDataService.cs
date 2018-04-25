namespace Lynx.Core.Interfaces
{
    public interface IPlatformSpecificDataService
    {
        int IDUID { get; set; }
        string IDAddress { get; set; }

        string GetDatabaseFile();

        IAccountService LoadAccount();
        void SaveAccount(IAccountService accountService);
    }
}