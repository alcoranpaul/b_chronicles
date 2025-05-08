using System;
using Bible;
using System.Threading;
using Utils;
using System.Threading.Tasks;

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
                    if (sessionManager.HasSessions())
                    {
                        Console.WriteLine("======");
                        Console.WriteLine("1. Continue");
                        Console.WriteLine("2. Main Menu");
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
                    else
                    {

                    }
                }

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
    static async Task ShowMainMenu()
    {
        ChangeState(State.MainMenu);

        MenuOptions option1 = new MenuOptions("Read the Bible", () => QueueTypingSessions(BibleBooks.Genesis, 1, 1));

        MenuOptions option2 = new MenuOptions("Exit", () =>
        {
            Console.WriteLine("👋 Goodbye!");
            ChangeState(State.End);
        });

        await ShowMenuAsync("Bible Typing App", option1, option2);




    }

    private static async Task ShowMenuAsync(string menuTitle, params MenuOptions[] options)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine($"=== {menuTitle} ===");

            // Display all menu options
            for (int i = 0; i < options.Length; i++)
            {
                Console.WriteLine($"[{i + 1}] {options[i].optionText}");
            }

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

                Thread.Sleep(3000);
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
    private enum State
    {
        MainMenu,
        TypingSession,
        End
    }

    private struct MenuOptions
    {
        public string optionText;
        public Action? action;

        public MenuOptions(string optionText, Action? action = null)
        {
            this.optionText = optionText;
            this.action = action;
        }
    }
}


