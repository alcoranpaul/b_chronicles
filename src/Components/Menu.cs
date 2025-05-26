
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
                options[choice - 1].Action?.Invoke();

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
                options[choice - 1].Action?.Invoke();
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

        Console.Write(message + " "); // Display the message
        char[] spinner = ['|', '/', '-', '\\']; // Characters for the spinning bar

        int totalIterations = durationInSeconds * 1000 / delay; // Calculate the total number of iterations

        for (int i = 0; i < totalIterations; i++)
        {
            Console.Write(spinner[i % spinner.Length]); // Display the current spinner character
            await Task.Delay(delay); // Pause asynchronously for the specified delay
            Console.Write("\b"); // Move the cursor back to overwrite the spinner character
        }

        Console.WriteLine(); // Move to the next line after the animation

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
        public Action? Action { get; }

        /// <summary>
        /// Initializes a new menu option.
        /// </summary>
        /// <param name="optionText">The text to display for this option.</param>
        /// <param name="action">The action to execute when selected (optional).</param>
        public Options(string optionText, Action? action = null)
        {
            OptionText = optionText;
            Action = action;
        }
    }
}