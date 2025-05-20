using Bible;
using Player;
using Utils;

namespace main;

static class Program
{
    private static readonly GameStateManager _stateManager = new();
    private static readonly TypingSessionManager sessionManager = new();
    private static readonly UnlockManager unlockManager = UnlockManager.Instance;
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


    private static async Task RunApp()
    {
        while (_stateManager.CurrentState != GameStateManager.State.End)
        {
            switch (_stateManager.CurrentState)
            {
                case GameStateManager.State.MainMenu:
                    await ShowMainMenu();
                    break;

                case GameStateManager.State.TypingSession:
                    await ProcessSessions();
                    break;

                case GameStateManager.State.Profile:
                    await ShowProfileMenu();
                    break;

                default:
                    Print("Unknown state. Returning to the main menu...");
                    _stateManager.ChangeState(GameStateManager.State.MainMenu);
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
            _stateManager.ChangeState(GameStateManager.State.MainMenu);
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
                _stateManager.ChangeState(GameStateManager.State.MainMenu);
            }
        }
    }

    private static async Task ShowMainMenu()
    {
        _stateManager.ChangeState(GameStateManager.State.MainMenu);

        Menu.Options readOption = new("Read the Bible", () =>
        {
            QueueTypingSessions(BibleBooks.Genesis, 1, 1);
            QueueTypingSessions(BibleBooks.Genesis, 1, 2);
            _stateManager.ChangeState(GameStateManager.State.TypingSession);
        });

        Menu.Options playerInfoOption = new("Profile", () =>
        {
            _stateManager.ChangeState(GameStateManager.State.Profile);
        });

        Menu.Options exitOption = new("Exit", () =>
        {
            LogDebug("Requested to end application");
            _stateManager.ChangeState(GameStateManager.State.End);
        });

        await Menu.Show("Bible Typing App", shouldClearPrev: true, readOption, playerInfoOption, exitOption);
    }

    private static async Task ShowProfileMenu()
    {
        Menu.Options option1 = new Menu.Options("Book Progression");
        Menu.Options option2 = new Menu.Options("Characters");
        Menu.Options option3 = new Menu.Options("Events");
        Menu.Options mainMenuOption = new Menu.Options("Main Menu", () =>
        {
            _stateManager.ChangeState(GameStateManager.State.MainMenu);
        });

        await Menu.Show("Profile", shouldClearPrev: true, option1, option2, option3, mainMenuOption);
    }

    private static async Task ShowContinueMenu()
    {
        Menu.Options option1 = new Menu.Options("Continue");

        Menu.Options option2 = new Menu.Options("Main Menu", () =>
        {
            _stateManager.ChangeState(GameStateManager.State.MainMenu);
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


    private static void RestoreConsoleSettings()
    {
        Console.CursorVisible = true; // Restore cursor visibility
    }



}