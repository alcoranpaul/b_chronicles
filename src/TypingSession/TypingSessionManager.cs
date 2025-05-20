using Bible;

public class TypingSessionManager
{
    private readonly Queue<SessionInfo> sessions;

    public TypingSessionManager()
    {
        sessions = new();
    }

    public void AddSession(SessionInfo session)
    {
        sessions.Enqueue(session);
    }

    public void AddSession(BibleBooks book, int chapter, int verse)
    {
        sessions.Enqueue(new SessionInfo(book, chapter, verse));
    }

    public bool RunNext()
    {
        if (sessions.Count == 0) return true;

        TypingSession current = sessions.Peek().session;
        switch (current.CurrentState)
        {
            case TypingSession.State.NotStarted:
                RenderSessionBookInfo(sessions.Peek());
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

    private static void RenderSessionBookInfo(SessionInfo current)
    {
        Console.Clear();

        Console.SetCursorPosition(0, 0);
        Console.ForegroundColor = ConsoleColor.Yellow;

        Console.WriteLine($"[{current.book} {current.chapter}:{current.verse}]");
        Console.ResetColor();
    }

    public bool HasSessions() => sessions.Count > 0;

    public struct SessionInfo
    {
        public BibleBooks book;
        public int chapter;
        public int verse;
        public TypingSession session;


        public SessionInfo(BibleBooks book, int chapter, int verse)
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
                Book b = Book.GetBook(book);
                string verseText = b.GetVerse(chapter, verse);
                return new TypingSession(verseText);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating session: {ex.Message}");
                throw;
            }
        }
    }
}
