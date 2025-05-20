namespace main;

public class GameStateManager : IStateChange
{
    public static GameStateManager Instance { get; private set; } = new();
    public State CurrentState { get; private set; }
    public event Action<State>? OnStateChanged;

    private GameStateManager()
    {
        CurrentState = State.MainMenu;
    }


    void IStateChange.ChangeState(State newState) => ChangeState(newState);
    internal void ChangeStateInternal(State newState) => ChangeState(newState);

    private void ChangeState(State newState)
    {
        if (newState == CurrentState) return;

        LogDebug($"State changed from {CurrentState} to {newState}");
        CurrentState = newState;
        OnStateChanged?.Invoke(newState);
    }


    public enum State
    {
        MainMenu,
        TypingSession,
        Profile,
        End
    }
}
