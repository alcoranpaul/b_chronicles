using System.Threading.Tasks;
using Bible;
using DataFetcher;
using Player;
using Utils;

namespace main;

static class Program
{
    private static BMain bmain;
    private static readonly FTMenu _ftMenu = new();
    static async Task Main()
    {
        try
        {
            ConfigureLogging();
            await FirstTimeMenu();
            // bmain = BMain.Instance;
            // await bmain.Run();
        }
        catch (Exception ex)
        {
            LogError($"Error has been cathced: {ex}");
        }

    }

    private static async Task FirstTimeMenu()
    {
        string jsonDir = Path.Combine("json", "player");
        if (!Directory.Exists(jsonDir))
        {
            LogDebug($"Application has launched for the first time!");
            await _ftMenu.ShowAsync();

        }

    }

    private static void ConfigureLogging()
    {
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Development");
        LoggingConfig.ConfigureLogging(true, false);
    }
}