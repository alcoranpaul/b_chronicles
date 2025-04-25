using System;
using Bible;
using Utils;

static class Program
{
    private static TypingSessionManager sessionManager = new();

    static void Main()
    {
        ConfigureLogging();
        Console.Clear();

        QueueTypingSessions();

        while (sessionManager.HasSessions())
        {
            sessionManager.RunNext();
        }

        Console.WriteLine("📖 All sessions complete!");
    }

    private static void QueueTypingSessions()
    {
        sessionManager.AddSession(CreateSession(BibleBooks.Genesis, 1, 1));
        sessionManager.AddSession(CreateSession(BibleBooks.Genesis, 1, 2));

    }

    private static TypingSession CreateSession(BibleBooks book, int chapter, int verse)
    {
        Book b = Book.GetBook(book);
        string verseText = b.GetVerse(chapter, verse);
        return new TypingSession(verseText);
    }

    private static void ConfigureLogging()
    {
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Development");
        LoggingConfig.ConfigureLogging(true, false);
    }
}
