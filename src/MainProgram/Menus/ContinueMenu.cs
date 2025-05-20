

using Bible;

namespace main;

public class ContinueMenu : IMenu
{
    private readonly IStateChange _stateManager;
    private readonly ISessionAdder _sessionManager;

    public ContinueMenu()
    {
        _stateManager = GameStateManager.Instance;
        _sessionManager = TypingSessionManager.Instance;
    }

    public async Task ShowAsync()
    {
        Menu.Options option1 = new Menu.Options("Continue");

        Menu.Options option2 = new Menu.Options("Main Menu", () =>
        {
            _stateManager.ChangeState(GameStateManager.State.MainMenu);
        });

        await Menu.Show("Would you like to continue?", shouldClearPrev: false, option1, option2);
    }
}