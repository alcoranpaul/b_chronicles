

using Bible;

namespace main;

public class MainMenu : Menu
{
    public override async Task ShowAsync()
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

        await Show("Bible Typing App", shouldClearPrev: true, readOption, playerInfoOption, exitOption);
    }
}