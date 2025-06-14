
namespace main;

/// <summary>
/// Abstract base class for all menu implementations in the application.
/// Provides common menu functionality like rendering, option selection, and loading animations.
/// </summary>
/// <remarks>
/// Inherit from this class to create specific menus (e.g. MainMenu, SettingsMenu).
/// </remarks>
public abstract class Menu : IMenu
{
    /// <summary>
    /// The state manager instance for handling application state transitions.
    /// </summary>
    protected readonly IStateChange _stateManager;

    /// <summary>
    /// The session manager instance for managing typing sessions.
    /// </summary>
    protected readonly ISessionAdder _sessionManager;

    private static readonly Random _random = new();

    public Menu()
    {
        _stateManager = GameStateManager.Instance;
        _sessionManager = TypingSessionManager.Instance;
    }

    /// <summary>
    /// Displays the menu and handles user interaction asynchronously.
    /// </summary>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public abstract Task ShowAsync();

    /// <summary>
    /// Displays a menu with the specified title and options.
    /// </summary>
    /// <param name="menuTitle">The title to display at the top of the menu.</param>
    /// <param name="shouldClearPrev">Whether to clear the console before rendering (default: true).</param>
    /// <param name="options">Array of menu options to display.</param>
    /// <returns>A Task representing the asynchronous menu operation.</returns>
    /// <example>
    /// <code>
    /// await Show("Main Menu", true, 
    ///     new Options("Start Game", StartGame), 
    ///     new Options("Exit", Exit));
    /// </code>
    /// </example>
    protected async Task Show(string menuTitle, bool shouldClearPrev = true, params Options[] options)
    {
        while (true)
        {
            if (shouldClearPrev)
                Console.Clear();

            string titleDecoration = new string('=', menuTitle.Length + 4);
            Print(titleDecoration);
            Print($"| {menuTitle} |");
            Print(titleDecoration + "\n");

            // Display all menu options
            for (int i = 0; i < options.Length; i++)
            {
                Print($"[{i + 1}] ", ConsoleColor.Cyan, false);
                Print(options[i].OptionText);
            }


            Console.WriteLine();

            // Read a single key press
            ConsoleKeyInfo key = Console.ReadKey(false);
            Console.Clear();
            // Convert the key to a number (if valid)
            int choice = key.KeyChar - '0'; // Convert the key character to an integer (e.g., '1' -> 1)

            if (choice >= 1 && choice <= options.Length)
            {
                // Execute the selected action
                await options[choice - 1].InvokeAsync();

                await ShowLoadingAnimationAsync("Loading", 2);
                Console.Clear();
                break; // Exit the menu loop after a valid selection
            }
            else
            {
                Print("\n❌ Invalid option.", ConsoleColor.Red);
                Print("\nTry again.");
                await ShowLoadingAnimationAsync("Returning to the menu", 2);

            }
        }
    }

    /// <summary>
    /// Displays a menu with the specified title, optional subtitles, and options.
    /// </summary>
    /// <param name="menuTitle">The title to display at the top of the menu (wrapped in ===).</param>
    /// <param name="shouldClearPrev">Whether to clear the console before rendering (default: true).</param>
    /// <param name="options">Array of menu options to display.</param>
    /// <param name="subtitles">Optional subtitles to display below the main title.</param>
    /// <returns>A Task representing the asynchronous menu operation.</returns>
    /// <example>
    /// <code>
    /// await Show("Main Menu", true, 
    ///     new Options("Start Game", StartGame), 
    ///     new Options("Exit", Exit),
    ///     "Version 1.0",
    ///     "Select an option below");
    /// </code>
    /// </example>
    protected async Task Show(string menuTitle, Options[] options, bool shouldClearPrev = true, params string[] subtitles)
    {
        while (true)
        {
            if (shouldClearPrev)
                Console.Clear();

            // Render title with === decoration
            string titleDecoration = new string('=', menuTitle.Length + 4);
            Print(titleDecoration);
            Print($"| {menuTitle} |");
            Print(titleDecoration + "\n");

            // Render optional subtitles
            if (subtitles.Length > 0)
            {
                foreach (string subtitle in subtitles)
                {
                    Print($" {subtitle}");
                }
                Print("\n");
            }

            // Display all menu options
            for (int i = 0; i < options.Length; i++)
            {
                Print($"[{i + 1}] ", ConsoleColor.Cyan, false);
                Print(options[i].OptionText);
            }

            Console.WriteLine();

            // Read user input
            ConsoleKeyInfo key = Console.ReadKey(false);
            Console.Clear();

            int choice = key.KeyChar - '0'; // Convert key to number

            if (choice >= 1 && choice <= options.Length)
            {
                await options[choice - 1].InvokeAsync();
                await ShowLoadingAnimationAsync("Loading", 2);
                Console.Clear();
                break;
            }
            else
            {
                Print("\n❌ Invalid option.", ConsoleColor.Red);
                Print("Try again.");
                await ShowLoadingAnimationAsync("Returning to the menu", 2);
            }
        }
    }



    /// <summary>
    /// Displays an animated loading spinner with a message.
    /// </summary>
    /// <param name="message">The message to display alongside the spinner.</param>
    /// <param name="durationInSeconds">Total duration of the animation in seconds (default: 3).</param>
    /// <param name="delay">Delay between animation frames in milliseconds (default: 100).</param>
    /// <returns>A Task representing the asynchronous animation.</returns>
    private async Task ShowLoadingAnimationAsync(string message, int durationInSeconds = 3, int delay = 100)
    {
        int choice = _random.Next(6); // Randomly pick one of 6 animations

        switch (choice)
        {
            case 0:
                await ShowSpinningBarAsync(message, durationInSeconds, delay);
                break;
            case 1:
                await ShowPacManAsync(message, durationInSeconds, delay);
                break;
            case 2:
                await ShowKaomojiWaveAsync(message, durationInSeconds, delay * 2);
                break;
            case 3:
                await ShowKatakanaSpinnerAsync(message, durationInSeconds, delay);
                break;
            case 4:
                await ShowFacesAsync(message, durationInSeconds, delay * 2);
                break;
            case 5:
                await ShowBouncingBallAsync(message, durationInSeconds, delay / 2);
                break;
        }
    }

    private async Task ShowSpinningBarAsync(string message, int durationSeconds, int delayMs)
    {
        Console.Write(message + " ");
        char[] spinner = { '|', '/', '-', '\\' };
        int totalIterations = durationSeconds * 1000 / delayMs;

        for (int i = 0; i < totalIterations; i++)
        {
            Console.Write(spinner[i % spinner.Length]);
            await Task.Delay(delayMs);
            Console.Write('\b');
        }
        Console.WriteLine();
    }

    private async Task ShowPacManAsync(string message, int durationSeconds, int delayMs)
    {
        Console.Write(message + " ");
        string[] frames = { "C", "◐", "◓", "◑", "◒" };
        int totalIterations = durationSeconds * 1000 / delayMs;
        int dotCount = 5;

        for (int i = 0; i < totalIterations; i++)
        {
            int frame = i % frames.Length;
            int dots = i % (dotCount + 1);

            Console.Write($"{frames[frame]}{new string('.', dots)}{new string(' ', dotCount - dots)}");
            await Task.Delay(delayMs);
            Console.Write('\r' + message + " ");
        }
        Console.WriteLine();
    }

    private async Task ShowKaomojiWaveAsync(string message, int durationSeconds, int delayMs)
    {
        Console.Write(message + " ");
        string[] frames = { "(^_^)/ ", "(^_^) ", "(^_^)/ ", "(^_^) ", "(^_^)/ " };
        int totalIterations = durationSeconds * 1000 / delayMs;

        for (int i = 0; i < totalIterations; i++)
        {
            string frame = frames[i % frames.Length];
            Console.Write(frame);
            await Task.Delay(delayMs);
            Console.Write(new string('\b', frames.Length));
        }
        Console.WriteLine();
    }

    private async Task ShowKatakanaSpinnerAsync(string message, int durationSeconds, int delayMs)
    {
        Console.Write(message + " ");
        char[] spinner = { 'ノ', 'ヽ', 'ヾ', 'ヿ', 'ヾ', 'ヽ' };
        int totalIterations = durationSeconds * 1000 / delayMs;

        for (int i = 0; i < totalIterations; i++)
        {
            Console.Write(spinner[i % spinner.Length]);
            await Task.Delay(delayMs);
            Console.Write(new string('\b', spinner.Length));

        }
        Console.WriteLine();
    }

    private async Task ShowFacesAsync(string message, int durationSeconds, int delayMs)
    {
        Console.Write(message + " ");
        string[] faces = { "(・_・)", "(^_^)", "(>_<)", "(^o^)", "(^_~)", "(o_O)" };
        int totalIterations = durationSeconds * 1000 / delayMs;

        for (int i = 0; i < totalIterations; i++)
        {
            string face = faces[i % faces.Length];
            Console.Write(face);
            await Task.Delay(delayMs);
            Console.Write(new string('\b', faces.Length));

        }
        Console.WriteLine();
    }

    private async Task ShowBouncingBallAsync(string message, int durationSeconds, int delayMs)
    {
        Console.Write(message + " ");
        int width = 10;
        int totalIterations = durationSeconds * 1000 / delayMs;

        for (int i = 0; i < totalIterations; i++)
        {
            int pos = i % (2 * width);
            int ballPos = pos <= width ? pos : 2 * width - pos;
            string line = new string(' ', ballPos) + "o" + new string(' ', width - ballPos);

            Console.Write(line);
            await Task.Delay(delayMs);
            Console.Write('\r' + message + " ");
        }
        Console.WriteLine();
    }
    /// <summary>
    /// Represents a single selectable menu option.
    /// </summary>
    /// <remarks>
    /// This nested type is only accessible to Menu and its derived classes.
    /// </remarks>
    protected struct Options
    {
        /// <summary>
        /// Gets the display text for this menu option.
        /// </summary>
        public string OptionText { get; }

        /// <summary>
        /// Gets the action to execute when this option is selected (optional).
        /// </summary>
        public Action? SyncAction { get; }

        public Func<Task>? AsyncAction { get; }

        public async Task InvokeAsync()
        {
            if (AsyncAction != null)
            {
                await AsyncAction.Invoke();
            }
            else
            {
                SyncAction?.Invoke();
            }
        }

        public Options(string optionText, Func<Task> asyncAction)
        {
            OptionText = optionText;
            AsyncAction = asyncAction;
            SyncAction = null;
        }

        /// <summary>
        /// Initializes a new menu option.
        /// </summary>
        /// <param name="optionText">The text to display for this option.</param>
        /// <param name="syncAction">The action to execute when selected (optional).</param>
        public Options(string optionText, Action? syncAction = null)
        {
            OptionText = optionText;
            SyncAction = syncAction;
        }
    }
}