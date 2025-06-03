

using System.Reflection;
using Bible;

namespace main;

public class SchedulerAddedMenu : Menu
{
    public override async Task ShowAsync()
    {
        Options back = new Options("Back", () =>
        {
            _stateManager.ChangeState(GameStateManager.State.Settings);
        });

        ClearConsole();
        StartupManager.AddAppToStartup();
        await Show("Windows Scheduler", [back], shouldClearPrev: false, []);
    }
}