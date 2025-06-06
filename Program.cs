using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Bible;
using DataFetcher;
using Player;
using Utils;

namespace main;

static class Program
{
    private static BMain? bmain;
    private static readonly FTMenu _ftMenu = new();

    static async Task Main()
    {
        try
        {
            Console.CursorVisible = false;
            Console.Clear();

            ConfigureLogging();

            await FirstTimeMenu();

            bmain = BMain.Instance;
            await bmain.Run();
        }
        catch (Exception ex)
        {
            LogError($"Error has been caught: {ex}");
        }
        finally
        {
            RestoreConsoleSettings();
        }
    }

    private static async Task FirstTimeMenu()
    {
        string jsonDir = PathDirHelper.GetBooksDirectory();

        if (!Directory.Exists(jsonDir))
        {
            LogInfo($"Application has launched for the first time!: {jsonDir}");
            Print($"jsonDir: {jsonDir}");
            await _ftMenu.ShowAsync();
        }
    }


    private static void ConfigureLogging()
    {
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Production");
        LoggingConfig.ConfigureLogging(true, false);
    }

    private static void RestoreConsoleSettings()
    {
        Console.CursorVisible = true; // Restore cursor visibility
    }
}