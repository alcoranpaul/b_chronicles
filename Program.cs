using System;
using Bible;
using System.Threading;
using Utils;

static class Program
{
    private static TypingSessionManager sessionManager = new();
    private static State state = State.MainMenu;

    static void Main()
    {
        ConfigureLogging();
        Console.Clear();
        ShowMainMenu();

        while (state != State.End)
        {
            if (sessionManager.HasSessions())
            {
                bool isSessionDone = sessionManager.RunNext();
                if (isSessionDone)
                {
                    Console.WriteLine("======");
                    Console.WriteLine("1. Continue");
                    Console.WriteLine("2. Exit");
                    Console.Write("Choose an option: ");
                    string input = Console.ReadLine() ?? string.Empty;
                    switch (input)
                    {
                        case "2":
                            ShowMainMenu();
                            break;
                        default:
                            Console.WriteLine("❌ Invalid option. Try again.");
                            break;
                    }
                    Console.Clear();
                }
            }
            else
            {
                ChangeState(State.End);
            }
        }


    }

    private static void QueueTypingSessions(BibleBooks book, int chapter, int verse)
    {
        sessionManager.AddSession(CreateSession(book, chapter, verse));
        ChangeState(State.TypingSession);
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

    private static void ChangeState(State newState)
    {
        if (newState == state) return;
        state = newState;
    }
    static void ShowMainMenu()
    {
        ChangeState(State.MainMenu);
        Console.Clear();

        while (true)
        {
            Console.WriteLine("=== Bible Typing App ===");
            Console.WriteLine("[1] Read the Bible");
            Console.WriteLine("[2] Exit");
            Console.Write("Choose an option: ");

            string input = Console.ReadLine() ?? string.Empty;
            Console.Clear();

            switch (input)
            {
                case "1":
                    QueueTypingSessions(BibleBooks.Genesis, 1, 1);
                    QueueTypingSessions(BibleBooks.Genesis, 1, 2);
                    return; // Proceed to main loop
                case "2":
                    Console.WriteLine("👋 Goodbye!");
                    ChangeState(State.End);
                    return;
                default:
                    Console.WriteLine("┗|｀O′|┛ Invalid option. Try again.");
                    ShowLoadingAnimation("Returning to the menu");
                    Thread.Sleep(2000);
                    Console.Clear();
                    break;
            }
        }
    }

    private static void ShowLoadingAnimation(string message, int dotCount = 3, int delay = 500)
    {
        Console.Write(message);
        for (int i = 0; i < dotCount; i++)
        {
            Thread.Sleep(delay); // Pause for the specified delay (in milliseconds)
            Console.Write(".");
        }
        Console.WriteLine(); // Move to the next line after the animation
    }
    private enum State
    {
        MainMenu,
        TypingSession,
        End
    }
}


