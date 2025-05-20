
using System.Text.Json;
using Bible;

namespace main;

public class UnlockManager
{
    public UnlockManager()
    {
        TypingSessionManager.OnSessionCompleted += OnSessionCompleted;
    }

    private void OnSessionCompleted(BibleBooks book, int chapter, int verse)
    {
        string fileName = GetUnlockJSONName(book, verse) + ".json";

        try
        {
            UnlockData unlockData = FindUnlockData(book, fileName);

            UnlockEntry? unlockable = GetUnlock(book, chapter, verse, unlockData);
        }
        catch (FileNotFoundException)
        {
            LogError($"({fileName}) Unlock file not found.");
        }
        catch (Exception ex)
        {
            LogError($"Unexpected error reading unlock file: {ex.Message}");
        }

    }

    private UnlockEntry GetUnlock(BibleBooks book, int chapter, int verse, UnlockData unlockData)
    {
        string targetKey = GetBookFullName(book, chapter, verse);

        foreach (var (verseRef, entry) in unlockData)
        {
            if (verseRef == targetKey)
            {
                if (entry == null)
                    throw new InvalidOperationException($"Unlock entry for '{targetKey}' is null.");

                return entry;
            }
        }

        throw new KeyNotFoundException($"Unlock entry for '{targetKey}' not found in the data.");
    }


    private static UnlockData FindUnlockData(BibleBooks book, string fileName)
    {
        string filePath = Path.Combine("json", "unlocks", $"{book.ToString().ToLower()}", fileName);

        string content = File.ReadAllText(filePath);  // Throws FileNotFoundException if not found
        JsonSerializerOptions? options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
        };

        UnlockData unlockData = JsonSerializer.Deserialize<UnlockData>(content, options)!;
        return unlockData;
    }

    public static void End()
    {

    }



    private string GetBookFullName(BibleBooks book, int chapter, int verse)
    {
        return $"{book.ToString().ToLower()} {chapter}:{verse}";
    }

    private string GetUnlockJSONName(BibleBooks book, int verse)
    {
        return $"unlocks_{book.ToString().ToLower()}_chapter_{verse}";
    }

    public class UnlockData : Dictionary<string, UnlockEntry>
    {
    }

    public class UnlockEntry
    {
        public List<Unlock> Unlocks { get; set; } = new();
    }

    public class Unlock
    {
        public UnlockType Type { get; set; }
        public string? Value { get; set; }
        public string? Character { get; set; }
        public List<string>? Traits { get; set; }
    }

    public enum UnlockType
    {
        Character,
        Trait,
        Event
    }

}