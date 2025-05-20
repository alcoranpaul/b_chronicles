namespace main;

public class GameStateManager
{
    public enum State
    {
        MainMenu,
        TypingSession,
        Profile,
        End
    }
    public State CurrentState { get; private set; }
    public event Action<State>? OnStateChanged;

    public void ChangeState(State newState)
    {
        if (newState == CurrentState) return;

        LogDebug($"State changed from {CurrentState} to {newState}");
        CurrentState = newState;
        OnStateChanged?.Invoke(newState);
    }

}
