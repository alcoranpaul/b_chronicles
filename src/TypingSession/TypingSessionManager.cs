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

    public bool RunNext()
    {
        if (sessions.Count == 0) return true;

        TypingSession current = sessions.Peek();
        switch (current.CurrentState)
        {
            case TypingSession.State.NotStarted:
                current.Initialize();
                break;
            case TypingSession.State.InProgress:
                current.RunStep();
                break;
            case TypingSession.State.Completed:
            case TypingSession.State.Cancelled:
                sessions.Dequeue();
                return true;
        }
        return false;
    }

    public bool HasSessions() => sessions.Count > 0;
}
