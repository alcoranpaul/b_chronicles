
namespace main;
public static class Menu
{
    public static async Task Show(string menuTitle, bool shouldClearPrev = true, params Options[] options)
    {
        while (true)
        {
            if (shouldClearPrev)
                Console.Clear();
            Console.WriteLine($"=== {menuTitle} ===");

            // Display all menu options
            for (int i = 0; i < options.Length; i++)
            {
                Console.WriteLine($"[{i + 1}] {options[i].optionText}");
            }

            Console.WriteLine();
            Console.Write("Choose an option: ");

            // Read a single key press
            ConsoleKeyInfo key = Console.ReadKey(false);
            Console.Clear();
            // Convert the key to a number (if valid)
            int choice = key.KeyChar - '0'; // Convert the key character to an integer (e.g., '1' -> 1)

            if (choice >= 1 && choice <= options.Length)
            {
                // Execute the selected action
                options[choice - 1].action?.Invoke();

                await ShowLoadingAnimationAsync("Loading", 2);
                Console.Clear();
                break; // Exit the menu loop after a valid selection
            }
            else
            {
                Console.WriteLine("\n❌ Invalid option. Try again.");
                await ShowLoadingAnimationAsync("Returning to the menu", 2);

            }
        }
    }

    private static async Task ShowLoadingAnimationAsync(string message, int durationInSeconds = 3, int delay = 100)
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
    public struct Options
    {
        public string optionText;
        public Action? action;

        public Options(string optionText, Action? action = null)
        {
            this.optionText = optionText;
            this.action = action;
        }
    }
}
