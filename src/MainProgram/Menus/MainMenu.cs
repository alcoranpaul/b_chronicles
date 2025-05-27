

using Bible;

namespace main;

public class MainMenu : Menu
{
    public override async Task ShowAsync()
    {
        _stateManager.ChangeState(GameStateManager.State.MainMenu);

        Options readOption = new("Read the Bible", () =>
        {
            (BookNames book, int chapter, int verse) = Player.User.Instance.RequestBibleReading();
            _sessionManager.AddSession(book, chapter, verse);
            _stateManager.ChangeState(newState: GameStateManager.State.TypingSession);
        });

        //     Options playerInfoOption = new("Profile", () =>
        //    {
        //        _stateManager.ChangeState(GameStateManager.State.Profile);
        //    });

        Options settingOption = new("Settings", () =>
    {
        _stateManager.ChangeState(GameStateManager.State.Settings);
    });

        Options exitOption = new("Exit", () =>
        {
            LogDebug("Requested to end application");
            _stateManager.ChangeState(GameStateManager.State.End);
        });

        await Show("Bible Typing App", shouldClearPrev: true, readOption, settingOption, exitOption);
    }
}