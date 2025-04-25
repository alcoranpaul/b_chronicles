public class TypingSession
{
    private TypingEngine engine;

    public State CurrentState { get; private set; }

    public TypingSession(string verse)
    {
        engine = new TypingEngine(verse);
        ChangeState(State.NotStarted);
    }

    public void Initialize()
    {
        engine.Initialize();

        // Prints the required text to the console
        var (display, _) = engine.GetDisplayText();
        ConsoleRenderer.RenderTypedText(display);
        ChangeState(State.InProgress);
    }

    public bool RunStep()
    {
        if (CurrentState == State.Completed) return true;


        ConsoleKeyInfo key = Console.ReadKey(true);
        engine.HandleKeyPress(key);
        var (display, completed) = engine.GetDisplayText();
        ConsoleRenderer.RenderTypedText(display);

        if (completed)
        {
            Console.WriteLine("\n\n✔ Verse complete!");
            ChangeState(State.Completed);
            return true;
        }

        return false;
    }

    private void ChangeState(State newState)
    {
        if (CurrentState == newState) return;

        CurrentState = newState;
    }

    public enum State
    {
        NotStarted,
        InProgress,
        Completed
    }
}
