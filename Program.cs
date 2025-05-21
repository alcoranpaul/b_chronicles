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
    private static readonly ProfileMenu _profileMenu = new();
    private static readonly ContinueMenu _continueMenu = new();
    private static readonly User _user = User.Instance;

    static async Task Main()
    {
        try
        {
            InitializeApp();
            await RunApp();

        }
        catch (Exception ex)
        {
            LogError($"Error has been cathced: {ex}");
        }

        RestoreConsoleSettings();
    }

    private static void InitializeApp()
    {
        Console.CursorVisible = false;
        ConfigureLogging();
        Console.Clear();
    }


    private static async Task RunApp()
    {
        while (_stateManager.CurrentState != GameStateManager.State.End)
        {
            switch (_stateManager.CurrentState)
            {
                case GameStateManager.State.MainMenu:
                    await _mainMenu.ShowAsync();
                    break;

                case GameStateManager.State.TypingSession:
                    await ProcessSessions();
                    break;

                case GameStateManager.State.Profile:
                    await _profileMenu.ShowAsync();
                    break;

                default:
                    Print("Unknown state. Returning to the main menu...");
                    _stateManager.ChangeStateInternal(GameStateManager.State.MainMenu);
                    break;
            }
        }

        End();
    }

    private static void End()
    {
        if (_user != null)
        {
            _user.End();
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
                await _continueMenu.ShowAsync();
            }
            else
            {
                Print("All sessions completed.");
                _stateManager.ChangeStateInternal(GameStateManager.State.MainMenu);
            }
        }
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