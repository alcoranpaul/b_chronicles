

using Bible;

namespace main;

public class CancelledMenu : Menu
{
    public override async Task ShowAsync()
    {

        Options mainMenuOption = new Options("Main Menu", () =>
        {
            _stateManager.ChangeState(GameStateManager.State.MainMenu);
        });


        await Show("You have cancelled a this session!", [mainMenuOption], shouldClearPrev: true);
    }
}