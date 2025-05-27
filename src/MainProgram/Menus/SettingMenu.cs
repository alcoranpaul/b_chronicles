

using Bible;

namespace main;

public class SettingsMenu : Menu
{
    public override async Task ShowAsync()
    {
        // TODO: consider to make a user account.
        Options option1 = new Options("Application Information", () =>
        {
            _stateManager.ChangeState(GameStateManager.State.AppInfo);
        });

        Options option2 = new Options("Add to Windows Scheduler", async () =>
        {
            SchedulerMenu schedulerMenu = new();
            await schedulerMenu.ShowAsync();
        });


        Options exitOption = new("Exit", () =>
       {
           LogDebug("Requested to end application");
           _stateManager.ChangeState(GameStateManager.State.End);
       });


        if (Utils.PlatformHelper.IsWindows())
            await Show("Settings", shouldClearPrev: true, option1, option2, exitOption);
        else
            await Show("Settings", shouldClearPrev: true, option1, exitOption);
    }
}