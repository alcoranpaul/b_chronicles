using Bible;

namespace main;

public class TypingSessionManager : ISessionAdder
{
    public static TypingSessionManager Instance { get; private set; } = new TypingSessionManager();
    private readonly Queue<SessionInfo> _sessions;
    public static event Action<BookNames, int, int>? OnSessionCompleted;
    public static event Action<BookNames, int, int>? OnSessionStarted;
    public static event Action<BookNames, int, int>? OnSessionCanceled;

    private TypingSessionManager()
    {
        _sessions = new();
    }

    // Explicit implementation - only accessible via ISessionAdder
    void ISessionAdder.AddSession(BookNames book, int chapter, int verse)
        => _sessions.Enqueue(new SessionInfo(book, chapter, verse));


    // Internal method (only accessible within the assembly)
    internal void AddSessionInternal(BookNames book, int chapter, int verse)
        => _sessions.Enqueue(new SessionInfo(book, chapter, verse));



    internal bool RunNext()
    {
        if (_sessions.Count == 0) return true;
        SessionInfo currentInfo = _sessions.Peek();
        TypingSession current = currentInfo.session;
        switch (current.CurrentState)
        {
            case TypingSession.State.NotStarted:
                RenderSessionBookInfo(currentInfo);
                current.Initialize();
                OnSessionStarted?.Invoke(currentInfo.book, currentInfo.chapter, currentInfo.verse);
                break;
            case TypingSession.State.InProgress:
                current.RunStep();
                break;
            case TypingSession.State.Completed:
                _sessions.Dequeue();
                OnSessionCompleted?.Invoke(currentInfo.book, currentInfo.chapter, currentInfo.verse);
                return true;
            case TypingSession.State.Cancelled:
                _sessions.Dequeue();
                OnSessionCanceled?.Invoke(currentInfo.book, currentInfo.chapter, currentInfo.verse);
                return true;
        }
        return false;
    }

    private static void RenderSessionBookInfo(SessionInfo current)
    {
        Console.Clear();

        Console.SetCursorPosition(0, 0);
        Console.ForegroundColor = ConsoleColor.Yellow;

        Console.WriteLine($"[{current.book} {current.chapter}:{current.verse}]");
        Console.ResetColor();
    }

    internal bool HasSessions() => _sessions.Count > 0;
    internal void End()
    {
        OnSessionCompleted = null;
    }
    public struct SessionInfo
    {
        public BookNames book;
        public int chapter;
        public int verse;
        public TypingSession session;


        public SessionInfo(BookNames book, int chapter, int verse)
        {
            this.book = book;
            this.chapter = chapter;
            this.verse = verse;
            session = CreateSession();
        }

        private readonly TypingSession CreateSession()
        {
            try
            {
                Book b = Player.User.Instance.RequestBibleReading();
                string verseText = b.GetVerse(chapter, verse);
                return new TypingSession(verseText);
            }
            catch (Exception ex)
            {
                LogWarning($"Error creating session: {ex.Message}");
                throw;
            }
        }
    }
}
