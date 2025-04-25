public static class ConsoleRenderer
{
    public static void RenderTypedText(List<(char ch, ConsoleColor color)> display)
    {
        Console.SetCursorPosition(0, 2);
        foreach (var (ch, color) in display)
        {
            Console.ForegroundColor = color;
            Console.Write(ch);
        }
        Console.ResetColor();
    }

    // public static void RenderCharacterProfile(Character character)
    // {
    //     Console.WriteLine($"\n{character.Name.ToUpper()} {(character.IsUnlocked ? "(Unlocked)" : "[Locked]")}");
    //     foreach (var trait in character.Traits.Values)
    //     {
    //         if (trait.Unlocked)
    //             Console.WriteLine($"- {trait.Name}: {trait.Description} ({trait.VerseKey})");
    //     }
    // }
}
