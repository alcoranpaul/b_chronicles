

using Bible;

namespace main;

public class ProfileMenu : IMenu
{
    private readonly IStateChange _stateManager;
    private readonly ISessionAdder _sessionManager;

    public ProfileMenu()
    {
        _stateManager = GameStateManager.Instance;
        _sessionManager = TypingSessionManager.Instance;
    }

    public async Task ShowAsync()
    {
        Menu.Options option1 = new Menu.Options("Book Progression");
        Menu.Options option2 = new Menu.Options("Characters");
        Menu.Options option3 = new Menu.Options("Events");
        Menu.Options mainMenuOption = new Menu.Options("Main Menu", () =>
        {
            _stateManager.ChangeState(GameStateManager.State.MainMenu);
        });

        await Menu.Show("Profile", shouldClearPrev: true, option1, option2, option3, mainMenuOption);
    }
}