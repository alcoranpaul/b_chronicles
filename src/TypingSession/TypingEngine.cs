public class TypingEngine
{
    public string OriginalText { get; private set; }
    public string NormalizedText { get; private set; }
    public string UserInput { get; private set; } = "";

    public TypingEngine(string target)
    {
        OriginalText = target;
        NormalizedText = NormalizeQuotes(target);
    }

    /// <summary>
    /// Initializes the typing engine.
    /// Prints the target text to the console.
    /// </summary>
    public void Initialize()
    {
        LogDebug($"TypingEngine initialized with target: {OriginalText}");
    }

    public void HandleKeyPress(ConsoleKeyInfo key)
    {
        if (key.Key == ConsoleKey.Backspace && UserInput.Length > 0)
            UserInput = UserInput[..^1];
        else if (!char.IsControl(key.KeyChar))
        {
            // Normalize the input character before comparing
            char normalizedInput = NormalizeQuotes(key.KeyChar.ToString())[0];

            // Only accept if we haven't reached the end
            if (UserInput.Length < NormalizedText.Length)
            {
                UserInput += normalizedInput;
            }
        }

    }

    public (List<(char, ConsoleColor)>, bool) GetDisplayText()
    {
        List<(char, ConsoleColor)> result = new();
        bool allCorrect = true;

        for (int i = 0; i < OriginalText.Length; i++)
        {
            char originalChar = OriginalText[i];
            char displayChar = originalChar; // Default: show original character (space stays invisible)

            if (i < UserInput.Length)
            {
                bool isCorrect = UserInput[i] == NormalizedText[i];

                // Handle space errors (ONLY show ␣ when user makes a mistake)
                if (originalChar == ' ' || UserInput[i] == ' ')
                {
                    if (originalChar != ' ' && UserInput[i] == ' ')
                    {
                        // Case 1: User typed an extra space (red ␣)
                        result.Add(('␣', ConsoleColor.Red));
                        allCorrect = false;
                        continue;
                    }
                    else if (originalChar == ' ' && UserInput[i] != ' ')
                    {
                        // Case 2: User missed a space (red ␣ at original position)
                        result.Add(('␣', ConsoleColor.Red));
                        allCorrect = false;
                        continue;
                    }
                    // Case 3: Correct space → leave as invisible (no ␣)
                }

                // Non-space or correct space
                result.Add((displayChar, isCorrect ? ConsoleColor.Green : ConsoleColor.Red));
                if (!isCorrect) allCorrect = false;
            }
            else
            {
                // Untyped characters (never show ␣ here, even for untyped spaces)
                result.Add((displayChar, ConsoleColor.DarkGray));
                if (originalChar == ' ') allCorrect = false; // Still track correctness
            }
        }

        return (result, allCorrect && UserInput.Length == NormalizedText.Length);
    }
    private string NormalizeQuotes(string text)
    {
        return text
            // Normalize quotes
            .Replace('\u201c', '"')   // “ → "
            .Replace('\u201d', '"')   // ” → "
            .Replace('\u2018', '\'')  // ‘ → '
            .Replace('\u2019', '\'')  // ’ → '
                                      // Normalize dashes
            .Replace('\u2013', '-')   // – → -
            .Replace('\u2014', '-')   // — → -
                                      // Normalize spaces and ellipsis
            .Replace('\u2026', '.')   // … → ... (or keep as-is if preferred)
                                      // Less common quotes
            .Replace('\u201A', '\'')  // ‚ → '
            .Replace('\u201E', '"');   // „ → "
    }
}
