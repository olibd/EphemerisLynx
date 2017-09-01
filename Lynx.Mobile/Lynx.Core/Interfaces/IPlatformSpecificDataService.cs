namespace Lynx.Core.Interfaces
{
    public interface IPlatformSpecificDataService
    {
        int IDUID { get; set; }

        string GetDatabaseFile();
    }
}