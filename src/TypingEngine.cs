public class TypingEngine
{
    public string TargetText { get; private set; }
    public string UserInput { get; private set; } = "";

    public TypingEngine(string target)
    {
        TargetText = target;
    }

    public void Initialize()
    {
        Console.WriteLine($"Type: {TargetText}");

    }

    public void HandleKeyPress(ConsoleKeyInfo key)
    {
        if (key.Key == ConsoleKey.Backspace && UserInput.Length > 0)
            UserInput = UserInput[..^1];
        else if (!char.IsControl(key.KeyChar) && UserInput.Length < TargetText.Length)
            UserInput += key.KeyChar;

    }

    public (List<(char, ConsoleColor)>, bool) GetDisplayText()
    {
        List<(char, ConsoleColor)>? result = new List<(char, ConsoleColor)>();


        for (int i = 0; i < TargetText.Length; i++)
        {
            if (i < UserInput.Length)
            {
                ConsoleColor color = UserInput[i] == TargetText[i] ? ConsoleColor.Green : ConsoleColor.Red;
                result.Add((TargetText[i], color));
            }
            else
            {
                result.Add((TargetText[i], ConsoleColor.DarkGray));
            }
        }

        return (result, UserInput == TargetText);
    }
}
