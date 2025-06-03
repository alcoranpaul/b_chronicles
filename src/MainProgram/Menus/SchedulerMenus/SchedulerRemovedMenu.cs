

using System.Reflection;
using Bible;

namespace main;

public class SchedulerRemovedMenu : Menu
{
    public override async Task ShowAsync()
    {

        Options back = new Options("Back", () =>
        {
            _stateManager.ChangeState(GameStateManager.State.Settings);
        });
        ClearConsole();
        StartupManager.RemoveAppFromStartup();
        await Show("Windows Scheduler", [back], shouldClearPrev: false);
    }
}