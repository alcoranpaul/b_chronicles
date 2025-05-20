using Bible;
using Player;
using Utils;

namespace main;

static class Program
{
    private static readonly TypingSessionManager sessionManager = new();
    private static readonly UnlockManager unlockManager = UnlockManager.Instance;
    private static State state = State.MainMenu;
    private static User? user;

    static async Task Main()
    {
        try
        {
            InitializeApp();

            await RunApp();

        }
        finally
        {
            RestoreConsoleSettings();
        }
    }

    private static void InitializeApp()
    {
        Console.CursorVisible = false;
        ConfigureLogging();
        Console.Clear();

        user = new();


        if (!user.HasBooks)
        {
            LogDebug($"User has no books.");

            user.AddBibleBook(BibleBooks.Genesis);
        }
    }

    private static void RestoreConsoleSettings()
    {
        Console.CursorVisible = true; // Restore cursor visibility
    }

    private static async Task RunApp()
    {
        while (state != State.End)
        {
            switch (state)
            {
                case State.MainMenu:
                    await ShowMainMenu();
                    break;

                case State.TypingSession:
                    await ProcessSessions();
                    break;

                default:
                    Print("Unknown state. Returning to the main menu...");
                    ChangeState(State.MainMenu);
                    break;
            }
        }

        OnEnded();
    }

    private static void OnEnded()
    {
        if (user != null)
        {
            user.End();
            TypingSessionManager.End();
            unlockManager.End();
        }
    }

    private static async Task ProcessSessions()
    {
        if (!sessionManager.HasSessions())
        {
            Print("No more sessions available.");
            ChangeState(State.MainMenu);
            return;
        }

        bool isInstanceSessionDone = sessionManager.RunNext();
        if (isInstanceSessionDone)
        {
            Print("\n\n✔ Verse complete!");
            // Has next sesison queued?
            if (sessionManager.HasSessions())
            {
                await ShowContinueMenu();
            }
            else
            {
                Print("All sessions completed.");
                ChangeState(State.MainMenu);
            }
        }
    }

    private static async Task ShowMainMenu()
    {
        ChangeState(State.MainMenu);

        Menu.Options option1 = new("Read the Bible", () =>
        {
            QueueTypingSessions(BibleBooks.Genesis, 1, 1);
            QueueTypingSessions(BibleBooks.Genesis, 1, 2);
            ChangeState(State.TypingSession);
        });

        Menu.Options option2 = new("Exit", () =>
        {
            LogDebug("Requested to end application");
            ChangeState(State.End);
        });

        await Menu.Show("Bible Typing App", shouldClearPrev: true, option1, option2);
    }

    private static async Task ShowContinueMenu()
    {
        Menu.Options option1 = new Menu.Options("Continue");

        Menu.Options option2 = new Menu.Options("Main Menu", () =>
        {
            ChangeState(State.MainMenu);
        });

        await Menu.Show("Would you like to continue?", shouldClearPrev: false, option1, option2);
    }

    private static void QueueTypingSessions(BibleBooks book, int chapter, int verse)
    {
        sessionManager.AddSession(book, chapter, verse);
    }

    private static void ConfigureLogging()
    {
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Development");
        LoggingConfig.ConfigureLogging(true, false);
    }

    private static void ChangeState(State newState)
    {
        if (newState == state) return;
        LogDebug($"State changed from {state} to {newState}");
        state = newState;
    }

    private enum State
    {
        MainMenu,
        TypingSession,
        End
    }
}