public class TypingSession
{
    private TypingEngine engine;

    public State CurrentState { get; private set; }

    public TypingSession(string textToType)
    {
        engine = new TypingEngine(textToType);
        ChangeState(State.NotStarted);
    }

    public void Initialize()
    {
        engine.Initialize();

        var (display, _) = engine.GetDisplayText();
        RenderTypedText(display);

        ChangeState(State.InProgress);
    }
    public bool RunStep()
    {
        if (CurrentState != State.InProgress) return true;


        ConsoleKeyInfo key = Console.ReadKey(true);
        if (key.Key == ConsoleKey.F2)
        {
            LogDebug("\n[F2] Triggered Session cancelled.");
            ChangeState(newState: State.Cancelled);
            return false;
        }
        engine.HandleKeyPress(key);
        var (display, completed) = engine.GetDisplayText();
        RenderTypedText(display);

        if (completed)
        {
            LogDebug($"Session Completed");
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

    private static void RenderTypedText(List<(char ch, ConsoleColor color)> display)
    {
        Console.SetCursorPosition(0, 3);
        foreach (var (ch, color) in display)
        {
            Console.ForegroundColor = color;

            Console.Write(ch);
        }
        Console.ResetColor();
    }


    public enum State
    {
        NotStarted,
        InProgress,
        Completed,
        Cancelled
    }
}
