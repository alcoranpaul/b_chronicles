public class TypingSessionManager
{
    private readonly Queue<TypingSession> sessions;

    public TypingSessionManager()
    {
        sessions = new();
    }

    public void AddSession(TypingSession session)
    {
        sessions.Enqueue(session);
    }

    public void RunNext()
    {
        if (sessions.Count == 0) return;

        TypingSession current = sessions.Peek();
        switch (current.CurrentState)
        {
            case TypingSession.State.NotStarted:
                current.Initialize();
                break;
            case TypingSession.State.InProgress:
                if (current.RunStep()) sessions.Dequeue();
                break;
            case TypingSession.State.Completed:
                sessions.Dequeue();
                break;
        }
    }

    public bool HasSessions() => sessions.Count > 0;
}
