using System.Diagnostics;

namespace Utils;

public static class PathDirHelper
{

    public static string GetAppDirectory()
    {
        // Try getting path from executable first
        string? exePath = Process.GetCurrentProcess().MainModule?.FileName;
        string? dirFromExe = exePath != null ? Path.GetDirectoryName(exePath) : null;

        if (!string.IsNullOrEmpty(dirFromExe))
        {
            return dirFromExe;
        }

        // Fallback to base directory (works for most cases, but may point to temp folder)
        string baseDir = AppDomain.CurrentDomain.BaseDirectory;
        return baseDir;
    }

    public static string GetBooksDirectory() => Path.Combine(GetAppDirectory(), "json", "books");
    public static string GetPlayerDirectory() => Path.Combine(GetAppDirectory(), "json", "player");
    public static string GetUnlocksDirectory() => Path.Combine(GetAppDirectory(), "json", "unlocks");

}