using Bible;
using Player;
using Utils;

namespace main;

static class Program
{
    private static readonly GameStateManager _stateManager = GameStateManager.Instance;
    private static readonly TypingSessionManager _sessionManager = TypingSessionManager.Instance;
    private static readonly UnlockManager _unlockManager = UnlockManager.Instance;
    private static readonly MainMenu _mainMenu = new();
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
                    _stateManager.ChangeStateInternal(GameStateManager.State.MainMenu);
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
            _sessionManager.End();
            _unlockManager.End();
        }
    }

    private static async Task ProcessSessions()
    {
        if (!_sessionManager.HasSessions())
        {
            Print("No more sessions available.");
            _stateManager.ChangeStateInternal(GameStateManager.State.MainMenu);
            return;
        }

        bool isInstanceSessionDone = _sessionManager.RunNext();
        if (isInstanceSessionDone)
        {
            Print("\n\n✔ Verse complete!");
            // Has next sesison queued?
            if (_sessionManager.HasSessions())
            {
                await ShowContinueMenu();
            }
            else
            {
                Print("All sessions completed.");
                _stateManager.ChangeStateInternal(GameStateManager.State.MainMenu);
            }
        }
    }

    private static async Task ShowMainMenu()
    {
        await _mainMenu.ShowAsync();
    }

    private static async Task ShowProfileMenu()
    {
        Menu.Options option1 = new Menu.Options("Book Progression");
        Menu.Options option2 = new Menu.Options("Characters");
        Menu.Options option3 = new Menu.Options("Events");
        Menu.Options mainMenuOption = new Menu.Options("Main Menu", () =>
        {
            _stateManager.ChangeStateInternal(GameStateManager.State.MainMenu);
        });

        await Menu.Show("Profile", shouldClearPrev: true, option1, option2, option3, mainMenuOption);
    }

    private static async Task ShowContinueMenu()
    {
        Menu.Options option1 = new Menu.Options("Continue");

        Menu.Options option2 = new Menu.Options("Main Menu", () =>
        {
            _stateManager.ChangeStateInternal(GameStateManager.State.MainMenu);
        });

        await Menu.Show("Would you like to continue?", shouldClearPrev: false, option1, option2);
    }

    private static void QueueTypingSessions(BibleBooks book, int chapter, int verse)
    {

        _sessionManager.AddSessionInternal(book, chapter, verse);
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