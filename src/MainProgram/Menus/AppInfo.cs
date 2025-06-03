

using System.Reflection;
using Bible;

namespace main;

public class AppInfo : Menu
{
    public override async Task ShowAsync()
    {

        Options option2 = new Options("Main Menu", () =>
        {
            _stateManager.ChangeState(GameStateManager.State.MainMenu);
        });

        Options option1 = new Options("Check for updates", AutoUpdater.CheckForUpdatesAsync);


        string appName = $"App: {Assembly.GetEntryAssembly()?.GetName().Name}";
        string appVersion = $"Version: {Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion}";

        // Strip metadata after '+'
        if (!string.IsNullOrEmpty(appVersion) && appVersion.Contains('+'))
        {
            appVersion = appVersion.Split('+')[0];
        }
        string developer = $"Developed by: {Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company}";


        await Show("Application Information", [option2, option1], shouldClearPrev: false, [appName, appVersion, developer]);
    }
}