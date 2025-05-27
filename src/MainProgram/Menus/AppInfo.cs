

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


        string appName = $"App: {Assembly.GetEntryAssembly()?.GetName().Name}";
        string appVersion = $"Version: {Assembly.GetEntryAssembly()?.GetName().Version}";
        string developer = $"Developed by: {Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company}";


        await Show("Application Information", [option2], shouldClearPrev: false, [appName, appVersion, developer]);
    }
}