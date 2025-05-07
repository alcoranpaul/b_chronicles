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

            ConsoleKeyInfo key = Console.ReadKey(true); // Read a single key without displaying it
            Console.Clear();
            switch (key.Key)
            {
                case ConsoleKey.D1:
                    Console.WriteLine("Starting Bible Reading...");
                    // Queue sessions, etc.
                    break;
                case ConsoleKey.D2:
                    Console.WriteLine("👋 Goodbye!");
                    ChangeState(State.End);
                    return;
                default:
                    Console.WriteLine("❌ Invalid option. Try again.");
                    ShowLoadingAnimation("Returning to the menu");
                    Thread.Sleep(1000);
                    Console.Clear();
                    ShowMainMenu(); // Recurse or loop depending on your structure
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


