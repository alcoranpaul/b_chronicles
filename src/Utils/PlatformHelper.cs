using System.Runtime.InteropServices;
namespace Utils;


public static class PlatformHelper
{
    public static bool IsWindows() => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    public static bool IsLinux() => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
    public static bool IsMacOS() => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
    public static bool IsFreeBSD() => RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD);

    public static string GetPlatformName()
    {
        if (IsWindows()) return "Windows";
        if (IsLinux()) return "Linux";
        if (IsMacOS()) return "macOS";
        if (IsFreeBSD()) return "FreeBSD";
        return "Unknown";
    }
}