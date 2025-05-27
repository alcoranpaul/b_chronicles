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
        try
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("AutoUpdater", "1.0"));

            string url = $"https://api.github.com/repos/{RepoOwner}/{RepoName}/releases/latest";
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();
            var release = JObject.Parse(json);
            var latestVersion = release["tag_name"]!.ToString().TrimStart('v');

            if (new Version(latestVersion) > new Version(CurrentVersion))
            {
                Console.WriteLine($"New version {latestVersion} available!");
                Console.WriteLine("Would you like to update now? (Y/N)");
                var input = Console.ReadLine()?.Trim().ToUpper();

                if (input == "Y")
                {
                    await DownloadAndUpdateAsync(release);
                }
            }
            else
            {
                Print($"Application is currently updated! ༼ つ ◕_◕ ༽つ\n");
            }
        }
        catch (Exception ex)
        {
            Print($"Update check failed: {ex.Message}");
        }
    }

    private static async Task DownloadAndUpdateAsync(JObject release)
    {
        // Find asset
        var asset = release["assets"]?.FirstOrDefault(a =>
            a["name"]!.ToString().EndsWith(".zip") ||
            a["name"]!.ToString().EndsWith(".exe"));

        if (asset == null)
        {
            Console.WriteLine("No suitable download found in release.");
            return;
        }

        string downloadUrl = asset["browser_download_url"]!.ToString();
        string fileName = asset["name"]!.ToString();
        string tempFile = Path.Combine(Path.GetTempPath(), fileName);

        Console.WriteLine("Downloading update...");

        // Download file with HttpClient
        using (var client = new HttpClient())
        using (var response = await client.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead))
        {
            response.EnsureSuccessStatusCode();
            await using var fs = new FileStream(tempFile, FileMode.Create, FileAccess.Write, FileShare.None);
            await response.Content.CopyToAsync(fs);
        }

        Console.WriteLine("Download complete. Preparing to update...");

        // Get the path of the currently running app
        string exeDir = AppContext.BaseDirectory;
        string exeName = Path.GetFileName(Environment.ProcessPath)!; // .NET 6+

        // Create update script
        string updateScript = Path.Combine(Path.GetTempPath(), "update.cmd");
        File.WriteAllText(updateScript, $@"
@echo off
timeout /t 1 /nobreak >nul
taskkill /f /im ""{exeName}"" >nul 2>&1
xcopy /y /q ""{tempFile}"" ""{exeDir}""
start """" ""{Path.Combine(exeDir, exeName)}""
del ""{updateScript}""
");

        // Launch the script and exit the current app
        Process.Start(new ProcessStartInfo
        {
            FileName = updateScript,
            WindowStyle = ProcessWindowStyle.Hidden,
            CreateNoWindow = true
        });

        Environment.Exit(0);
    }

}