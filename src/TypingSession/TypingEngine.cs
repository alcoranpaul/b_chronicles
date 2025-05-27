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

        for (int i = 0; i < OriginalText.Length; i++)
        {
            if (i < UserInput.Length)
            {
                // Compare against normalized text but show original characters
                bool isCorrect = UserInput[i] == NormalizedText[i];
                result.Add((OriginalText[i], isCorrect ? ConsoleColor.Green : ConsoleColor.Red));
            }
            else
            {
                result.Add((OriginalText[i], ConsoleColor.DarkGray));
            }
        }

        return (result, UserInput == NormalizedText);
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
