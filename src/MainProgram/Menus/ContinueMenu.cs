

using Bible;

namespace main;

public class ContinueMenu : Menu
{
    public override async Task ShowAsync()
    {
        Options option1 = new Options("Continue");

        Options option2 = new Options("Main Menu", () =>
        {
            _stateManager.ChangeState(GameStateManager.State.MainMenu);
        });

        await Show("Would you like to continue?", shouldClearPrev: false, option1, option2);
    }
}