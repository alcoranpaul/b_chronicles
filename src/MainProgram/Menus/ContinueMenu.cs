

using Bible;

namespace main;

public class ContinueMenu : Menu
{
    public override async Task ShowAsync()
    {
        Options option1 = new Options("Continue", () =>
        {
            (BookNames book, int chapter, int verse) = Player.User.Instance.RequestBibleReading();
            _sessionManager.AddSession(book, chapter, verse);
            _stateManager.ChangeState(GameStateManager.State.TypingSession);
        });

        Options option2 = new Options("Main Menu", () =>
        {
            _stateManager.ChangeState(GameStateManager.State.MainMenu);
        });

        await Show("Would you like to continue?", shouldClearPrev: false, option1, option2);
    }
}