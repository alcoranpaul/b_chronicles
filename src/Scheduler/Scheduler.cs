#nullable enable
using System;
using System.Diagnostics;
using Microsoft.Win32;

namespace main;

public static class StartupManager
{
    private static readonly string AppName = "BibleChronicles";
    private static readonly string RunKeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

#if WINDOWS
    public static void AddAppToStartup()
    {
        string? exePath = Process.GetCurrentProcess().MainModule?.FileName;

        if (exePath == null || string.IsNullOrEmpty(exePath))
        {
            LogError($"AddAppToStartup: exePath is null");
            return;
        }

        try
        {
            using RegistryKey? reg = Registry.CurrentUser.OpenSubKey(RunKeyPath, true);
            if (reg?.GetValue(AppName) == null)
            {
                reg?.SetValue(AppName, $"\"{exePath}\"");
                LogInfo($"App added to startup");
                Print("App added to startup.");
            }
            else
            {
                LogWarning("App is already set to run at startup.");
                Print("App is already set to run at startup.");
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
            using RegistryKey? reg = Registry.CurrentUser.OpenSubKey(RunKeyPath, true);
            if (reg?.GetValue(AppName) != null)
            {
                reg?.DeleteValue(AppName);
                LogInfo("App removed from startup.");
                Print("App removed from startup.");
            }
            else
            {
                LogWarning("App was not set to run at startup.");
                Print("App was not set to run at startup.");
            }
        }
        catch (Exception ex)
        {
            LogError("Failed to remove from startup: " + ex.Message);
        }
    }
#else
    // No-op stubs for non-Windows platforms
    public static void AddAppToStartup()
    {
        LogInfo("Startup management is only supported on Windows.");
        Print("Startup management is only supported on Windows.");
    }

    public static void RemoveAppFromStartup()
    {
        LogInfo("Startup management is only supported on Windows.");
        Print("Startup management is only supported on Windows.");
    }
#endif

    // Dummy implementations of Log/Print methods for context
    private static void LogInfo(string message) => Console.WriteLine(message);
    private static void LogError(string message) => Console.Error.WriteLine(message);
    private static void LogWarning(string message) => Console.WriteLine("WARNING: " + message);
    private static void Print(string message) => Console.WriteLine(message);
}