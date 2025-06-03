using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection;
using Newtonsoft.Json.Linq;


public class AutoUpdater
{
    private const string RepoOwner = "alcoranpaul";
    private const string RepoName = "b_chronicles";
    private static string CurrentVersion = $"{Assembly.GetEntryAssembly()?.GetName().Version}"; // Match your AssemblyInfo.cs

    public static async Task CheckForUpdatesAsync()
    {
        LogInfo("AutoUpdater started.");
        try
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("AutoUpdater", "1.0"));

            string url = $"https://api.github.com/repos/{RepoOwner}/{RepoName}/releases/latest";
            LogInfo($"Fetching latest release from {url}");

            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();
            LogInfo("Received release info.");

            var release = JObject.Parse(json);
            var tagName = release["tag_name"]?.ToString() ?? "0.0.0";
            var latestVersion = tagName.TrimStart('v');

            LogInfo($"Current version: {CurrentVersion}, Latest version: {latestVersion}");

            if (Version.TryParse(latestVersion, out var latest) &&
                Version.TryParse(CurrentVersion, out var current))
            {
                if (latest > current)
                {
                    LogInfo("New version available.");
                    Console.WriteLine($"New version {latest} available!");
                    Console.WriteLine("Would you like to update now? (Y/N)");
                    var input = Console.ReadLine()?.Trim().ToUpper();

                    if (input == "Y")
                    {
                        await DownloadAndUpdateAsync(release);
                    }
                }
                else
                {
                    LogInfo("Application is up to date.");
                    Console.WriteLine("Application is currently updated! ༼ つ ◕_◕ ༽つ");
                }
            }
            else
            {
                LogError($"Failed to parse version numbers. Latest: {latestVersion}, Current: {CurrentVersion}");
                Console.WriteLine("Error parsing version numbers.");
            }
        }
        catch (Exception ex)
        {
            LogError($"Update check failed: {ex.Message}\n{ex.StackTrace}");
            Console.WriteLine($"Update check failed: {ex.Message}");
        }
    }

    private static async Task DownloadAndUpdateAsync(JObject release)
    {
        try
        {
            // 1. Detect current platform
            string platformTag;
            if (OperatingSystem.IsWindows())
            {
                platformTag = "win-x64";
            }
            else if (OperatingSystem.IsLinux())
            {
                platformTag = "linux-x64";
            }
            else if (OperatingSystem.IsMacOS())
            {
                platformTag = "osx-x64"; // fallback
            }
            else
            {
                LogError("Unsupported operating system.");
                Console.WriteLine("Unsupported operating system.");
                return;
            }

            LogInfo($"Detected platform: {platformTag}");

            // 2. Find matching asset
            var assets = release["assets"] as JArray;
            var asset = assets?
                .FirstOrDefault(a => a["name"]?.ToString().Contains(platformTag, StringComparison.OrdinalIgnoreCase) == true &&
                                     (a["name"]?.ToString().EndsWith(".zip", StringComparison.OrdinalIgnoreCase) == true ||
                                      a["name"]?.ToString().EndsWith(".exe", StringComparison.OrdinalIgnoreCase) == true));

            if (asset == null)
            {
                LogError($"No suitable download found for platform '{platformTag}'.");
                Console.WriteLine($"No update available for your platform ({platformTag}).");
                return;
            }

            string downloadUrl = asset["browser_download_url"]!.ToString();
            string fileName = asset["name"]!.ToString();

            LogInfo($"Found update file: {fileName} from {downloadUrl}");

            // 3. Use a stable folder instead of AppDomain temp
            string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string appNameFolder = "BChronicles";
            string stableUpdatePath = Path.Combine(appDataFolder, appNameFolder, "update");

            Directory.CreateDirectory(stableUpdatePath); // Ensures folder exists

            string tempFile = Path.Combine(stableUpdatePath, fileName);
            string updateScript = Path.Combine(stableUpdatePath, "update.cmd");
            string logFile = Path.Combine(stableUpdatePath, "update.log");

            LogInfo($"Downloading update to: {tempFile}");
            Console.WriteLine("Downloading update...");

            using (var client = new HttpClient())
            {
                // ✅ Set proper User-Agent header
                client.DefaultRequestHeaders.UserAgent.ParseAdd("BibleChronicles-Updater/1.0");

                using (var response = await client.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead))
                {
                    response.EnsureSuccessStatusCode();
                    await using var fs = new FileStream(tempFile, FileMode.Create, FileAccess.Write, FileShare.None);
                    await response.Content.CopyToAsync(fs);
                }
            }

            LogInfo("Download complete.");

            Console.WriteLine("Download complete. Preparing to update...");

            string exeName = Process.GetCurrentProcess().MainModule?.FileName;
            if (exeName == null)
            {
                LogError("Failed to detect application name.");
                return;
            }

            string exeDir = Path.GetDirectoryName(exeName);
            if (string.IsNullOrEmpty(exeDir))
            {
                LogError("Failed to detect application directory.");
                return;
            }
            LogInfo($"ExeName: {exeName}");
            LogInfo($"ExeDir: {exeDir}");
            string scriptContent = $@"
@echo off
:: Log file
set LOG={logFile}

echo [{DateTime.Now}] Starting update process >> %LOG%

:: Wait briefly
timeout /t 2 /nobreak >nul

echo [{DateTime.Now}] Killing running application: {exeName} >> %LOG%
taskkill /f /im ""{exeName}"" >nul 2>&1

:: Define paths
set ZIP_FILE={tempFile}
set TARGET_DIR={exeDir}
set EXTRACT_DIR={Path.Combine(stableUpdatePath, "extracted")}

echo [{DateTime.Now}] Creating extraction directory: %EXTRACT_DIR% >> %LOG%
if exist ""%EXTRACT_DIR%"" rd /s /q ""%EXTRACT_DIR%""
md ""%EXTRACT_DIR%""

echo [{DateTime.Now}] Extracting ZIP file to %EXTRACT_DIR% >> %LOG%
powershell -Command ""Expand-Archive -Path '%ZIP_FILE%' -DestinationPath '%EXTRACT_DIR%' -Force""

if errorlevel 1 (
    echo [{DateTime.Now}] ERROR: Failed to extract ZIP file >> %LOG%
    pause
    exit /b 1
)

echo [{DateTime.Now}] Copying updated files to %TARGET_DIR% >> %LOG%
xcopy /y /e /i ""%EXTRACT_DIR%"" ""%TARGET_DIR%"" >> %LOG% 2>&1

if errorlevel 1 (
    echo [{DateTime.Now}] ERROR: File copy failed >> %LOG%
    pause
    exit /b 1
)

echo [{DateTime.Now}] Cleaning up update files >> %LOG%
rd /s /q ""%EXTRACT_DIR%""

:: del ""%0""
";

            File.WriteAllText(updateScript, scriptContent);
            LogInfo($"Created update script at: {updateScript}");

            Process.Start(new ProcessStartInfo
            {
                FileName = updateScript,
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                UseShellExecute = true
            });

            LogInfo("Launching update script. Exiting application.");
            Environment.Exit(0);
        }
        catch (Exception ex)
        {
            LogError($"Update failed: {ex.Message}\n{ex.StackTrace}");
            Console.WriteLine($"Update failed: {ex.Message}");
        }
    }

}