

using Bible;

namespace main;

public class ProfileMenu : Menu
{


    public override async Task ShowAsync()
    {
        Options option1 = new Options("Book Progression");
        Options option2 = new Options("Characters");
        Options option3 = new Options("Events");
        Options mainMenuOption = new Options("Main Menu", () =>
        {
            _stateManager.ChangeState(GameStateManager.State.MainMenu);
        });

        await Show("Profile", shouldClearPrev: true, option1, option2, option3, mainMenuOption);
    }
}