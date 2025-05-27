

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

        Options exitOption = new("Exit", () =>
       {
           LogDebug("Requested to end application");
           _stateManager.ChangeState(GameStateManager.State.End);
       });



        await Show("Settings", shouldClearPrev: true, option1, exitOption);
    }
}