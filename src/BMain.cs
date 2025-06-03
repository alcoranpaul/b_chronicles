using System.Runtime.InteropServices;
using Player;
using Utils;

namespace main;

public class BMain
{
    public static BMain Instance { get; private set; } = new();
    private GameStateManager _stateManager;
    private TypingSessionManager _sessionManager;
    private UnlockManager _unlockManager;

    private readonly MainMenu _mainMenu = new();
    private readonly ProfileMenu _profileMenu = new();
    private readonly ContinueMenu _continueReadingMenu = new();
    private readonly CancelledMenu _cancelledReadingMenu = new();
    private readonly SettingsMenu _settingsMenu = new();
    private readonly AppInfo _appInfo = new();

    [DllImport("kernel32.dll")]
    private static extern bool SetShutdownHandler(HandlerRoutine handler, bool add);
    private delegate bool HandlerRoutine(uint dwCtrlType);



    private readonly User _user = User.Instance;

    private BMain()
    {
        InitializeApp();
        _stateManager = GameStateManager.Instance;
        _sessionManager = TypingSessionManager.Instance;
        _unlockManager = UnlockManager.Instance;
    }

    public async Task Run()
    {
        try
        {

            await RunApp();
        }
        catch (Exception ex)
        {
            LogError($"Error has been cathced: {ex}");
        }

        RestoreConsoleSettings();
    }

    private void InitializeApp()
    {
        Console.CursorVisible = false;
        SetShutdownHandler(ShutdownHandler, true);
        Console.Clear();
    }




    private async Task RunApp()
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

                case GameStateManager.State.AppInfo:
                    await _appInfo.ShowAsync();
                    break;

                case GameStateManager.State.Settings:
                    await _settingsMenu.ShowAsync();
                    break;

                default:
                    Print("Unknown state. Returning to the main menu...");
                    _stateManager.ChangeStateInternal(GameStateManager.State.MainMenu);
                    break;
            }
        }

        End();
    }

    private void End()
    {
        LogInfo($"Program Terminated.");

        if (_user != null)
        {
            _user.End();
            _sessionManager.End();
            _unlockManager.End();
        }
    }

    private async Task ProcessSessions()
    {
        if (!_sessionManager.HasSessions())
        {
            Print("No more sessions available.");
            _stateManager.ChangeStateInternal(GameStateManager.State.MainMenu);
            return;
        }

        bool isInstanceSessionDone = _sessionManager.RunNext(out TypingSessionManager.SessionState sessionState);
        if (isInstanceSessionDone)
        {
            switch (sessionState)
            {
                case TypingSessionManager.SessionState.Completed:

                    await _continueReadingMenu.ShowAsync();
                    break;

                case TypingSessionManager.SessionState.Cancelled:
                    await _cancelledReadingMenu.ShowAsync();
                    break;

                case TypingSessionManager.SessionState.Error:
                    LogError($"An Error has occured in the Typing Session!!!");
                    break;

            }
        }

    }




    /// <summary>
    /// Handles console closing events. Ends the program and returns true to
    /// prevent the default action of displaying an error message.
    /// </summary>
    /// <param name="dwCtrlType">The type of control signal.</param>
    /// <returns>true</returns>
    private bool ShutdownHandler(uint dwCtrlType)
    {
        End();
        return true;
    }


    private void RestoreConsoleSettings()
    {
        Console.CursorVisible = true; // Restore cursor visibility
    }

}