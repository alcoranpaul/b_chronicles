using Microsoft.Win32;
namespace main;

public static class StartupManager
{
    private static readonly string AppName = "BibleChronicles";
    private static readonly string RunKeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

    public static void AddAppToStartup()
    {
        string exePath = Environment.ProcessPath!;

        if (exePath == null || string.IsNullOrEmpty(exePath))
        {
            LogError($"AddAppToStartup: exePath is null");
            return;
        }

        try
        {
            using (RegistryKey reg = Registry.CurrentUser.OpenSubKey(RunKeyPath, true))
            {
                if (reg.GetValue(AppName) == null)
                {
                    reg.SetValue(AppName, $"\"{exePath}\"");
                    LogInfo("App added to startup.");
                    Print("App added to startup.");
                }
                else
                {
                    LogWarning("App is already set to run at startup.");
                    Print("App is already set to run at startup.");
                }
            }
        }
        catch (Exception ex)
        {
            LogError("Failed to set startup: " + ex.Message);
        }
    }

    public static void RemoveAppFromStartup()
    {
        try
        {
            using (RegistryKey reg = Registry.CurrentUser.OpenSubKey(RunKeyPath, true))
            {
                if (reg.GetValue(AppName) != null)
                {
                    reg.DeleteValue(AppName);
                    LogInfo("App removed from startup.");
                    Print("App removed from startup.");
                }
                else
                {
                    LogWarning("App was not set to run at startup.");
                    Print("App was not set to run at startup.");
                }
            }
        }
        catch (Exception ex)
        {
            LogError("Failed to remove from startup: " + ex.Message);
        }
    }
}