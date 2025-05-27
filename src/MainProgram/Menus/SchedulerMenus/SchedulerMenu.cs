

using System.Reflection;
using Bible;

namespace main;

public class SchedulerMenu : Menu
{
    public override async Task ShowAsync()
    {
        SchedulerAddedMenu addedMenu = new();
        SchedulerRemovedMenu removedMenu = new();

        Options option1 = new Options("Yes", addedMenu.ShowAsync);
        Options option2 = new Options("No", removedMenu.ShowAsync);

        Options back = new Options("Back", () =>
        {
            _stateManager.ChangeState(GameStateManager.State.Settings);
        });

        await Show("Windows Scheduler Settings", [option1, option2, back], shouldClearPrev: true);
    }
}