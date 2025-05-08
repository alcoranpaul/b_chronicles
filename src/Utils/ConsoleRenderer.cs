public static class ConsoleRenderer
{
    public static void RenderTypedText(List<(char ch, ConsoleColor color)> display)
    {
        Console.SetCursorPosition(0, 1);
        foreach (var (ch, color) in display)
        {
            Console.ForegroundColor = color;

            Console.Write(ch);
        }
        Console.ResetColor();
    }


}
