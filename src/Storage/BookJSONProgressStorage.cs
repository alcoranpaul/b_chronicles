using System.Text.Json;
using System.Text.Json.Serialization;

namespace Player.BibleBook;

public sealed class JsonProgressStorage : IBookProgressStorage
{
    private readonly string _filePath;
    private ProgressData? _cachedProgress;
    private readonly JsonSerializerOptions _jsonOptions;

    public JsonProgressStorage()
    {
        // Match your existing path structure
        _filePath = Path.Combine(
            Directory.GetParent(AppContext.BaseDirectory)!.Parent!.Parent!.Parent!.FullName,
            "json", "player", "bible_progress.json"
        );

        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() } // Enum-as-strings like your original
        };
    }

    public void BeginSession(Bible.BookNames book, int chapter, int verse)
    {
        _cachedProgress = new ProgressData(
            book, chapter, verse,
            NextBook: book, NextChapter: chapter, NextVerse: verse + 1
        );
        SaveToFile();
    }

    public void SaveProgress(Bible.BookNames book, int chapter, int verse,
        (Bible.BookNames nextBook, int nextChapter, int nextVerse) next)
    {
        _cachedProgress = new ProgressData(
            book, chapter, verse,
            next.nextBook, next.nextChapter, next.nextVerse,
            LastRead: DateTime.UtcNow
        );
        SaveToFile();
    }

    private void SaveToFile()
    {
        try
        {
            LogDebug($"Saving Bible progress to [{Path.GetFullPath(_filePath)}]");
            Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
            File.WriteAllText(_filePath, JsonSerializer.Serialize(_cachedProgress, _jsonOptions));
        }
        catch (Exception ex)
        {
            LogError($"Failed to save progress: {ex.Message}");
        }
    }

    public (Bible.BookNames book, int chapter, int verse)? LoadProgress()
    {
        if (!File.Exists(_filePath)) return null;

        try
        {
            string? json = File.ReadAllText(_filePath);
            _cachedProgress = JsonSerializer.Deserialize<ProgressData>(json, _jsonOptions);
            if (_cachedProgress!.LastRead != null)
            {
                LogDebug($"Loading next chapter and verse");
                return (_cachedProgress!.NextBook, _cachedProgress.NextChapter, _cachedProgress.NextVerse);
            }

            return (_cachedProgress!.CurrentBook, _cachedProgress.CurrentChapter, _cachedProgress.CurrentVerse);
        }
        catch (Exception ex)
        {
            LogError($"Failed to load progress: {ex.Message}");
            return null;
        }
    }

    public void Flush() => SaveToFile();

    private record ProgressData(
        Bible.BookNames CurrentBook,
        int CurrentChapter,
        int CurrentVerse,
        Bible.BookNames NextBook,
        int NextChapter,
        int NextVerse,
        DateTime? LastRead = null
    );
}