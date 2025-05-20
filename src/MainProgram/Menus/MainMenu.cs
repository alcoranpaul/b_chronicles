

using Bible;

namespace main;

public class MainMenu : IMenu
{
    private readonly IStateChange _stateManager;
    private readonly ISessionAdder _sessionManager;

    public MainMenu()
    {
        _stateManager = GameStateManager.Instance;
        _sessionManager = TypingSessionManager.Instance;
    }

    public async Task ShowAsync()
    {
        _stateManager.ChangeState(GameStateManager.State.MainMenu);

        Menu.Options readOption = new("Read the Bible", () =>
        {
            _sessionManager.AddSession(BibleBooks.Genesis, 1, 1);
            _sessionManager.AddSession(BibleBooks.Genesis, 1, 2);
            _stateManager.ChangeState(GameStateManager.State.TypingSession);
        });

        Menu.Options playerInfoOption = new("Profile", () =>
       {
           _stateManager.ChangeState(GameStateManager.State.Profile);
       });

        Menu.Options exitOption = new("Exit", () =>
        {
            LogDebug("Requested to end application");
            _stateManager.ChangeState(GameStateManager.State.End);
        });

        await Menu.Show("Bible Typing App", shouldClearPrev: true, readOption, playerInfoOption, exitOption);
    }
}