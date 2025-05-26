using System.Text.Json;
using System.Text.Json.Serialization;
using Bible;

namespace Player.BibleBook;

public sealed class BookJsonProgressStorage : IBookProgressStorage
{
    private readonly string _filePath;
    private ProgressData _cachedProgress;
    private readonly JsonSerializerOptions _jsonOptions;

    public BookJsonProgressStorage()
    {
        _filePath = Path.Combine(
            Directory.GetParent(AppContext.BaseDirectory)!.Parent!.Parent!.Parent!.FullName,
            "json", "player", "bible_progress.json"  // Renamed to be more generic
        );

        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        };

        _cachedProgress = new ProgressData();
    }

    public void BeginSession(BookNames book, int chapter, int verse)
    {
        _cachedProgress = new ProgressData
        {
            CurrentBook = book,
            CurrentChapter = chapter,
            CurrentVerse = verse,
            NextBook = book,
            NextChapter = chapter,
            NextVerse = verse + 1
        };

        // Example of storing additional data
        _cachedProgress.Data["LastSessionStart"] = DateTime.UtcNow;
        _cachedProgress.Data["SessionCount"] = GetSessionCount() + 1;

        SaveToFile();
    }

    public void SaveProgress(BookNames book, int chapter, int verse,
        (BookNames nextBook, int nextChapter, int nextVerse) next)
    {
        _cachedProgress = new ProgressData
        {
            CurrentBook = book,
            CurrentChapter = chapter,
            CurrentVerse = verse,
            NextBook = next.nextBook,
            NextChapter = next.nextChapter,
            NextVerse = next.nextVerse,
            LastRead = DateTime.UtcNow
        };

        SaveToFile();
    }

    // New method to store custom data
    public void StoreCustomData(string key, object value)
    {
        _cachedProgress.Data[key] = value;
        SaveToFile();
    }

    // New method to retrieve custom data
    public T? GetCustomData<T>(string key)
    {
        if (_cachedProgress.Data.TryGetValue(key, out var value))
        {
            return (T)value;
        }
        return default;
    }

    private int GetSessionCount()
    {
        return _cachedProgress.Data.TryGetValue("SessionCount", out var count)
            ? (int)count : 0;
    }

    private void SaveToFile()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
            File.WriteAllText(_filePath, JsonSerializer.Serialize(_cachedProgress, _jsonOptions));
        }
        catch (Exception ex)
        {
            LogError($"Failed to save progress: {ex.Message}");
        }
    }

    public (BookNames book, int chapter, int verse)? LoadProgress()
    {
        if (!File.Exists(_filePath)) return null;

        try
        {
            string json = File.ReadAllText(_filePath);
            _cachedProgress = JsonSerializer.Deserialize<ProgressData>(json, _jsonOptions)
                ?? new ProgressData();

            return (_cachedProgress.CurrentBook,
                   _cachedProgress.CurrentChapter,
                   _cachedProgress.CurrentVerse);
        }
        catch (Exception ex)
        {
            LogError($"Failed to load progress: {ex.Message}");
            return null;
        }
    }

    public void Flush() => SaveToFile();



    private record ProgressData
    {
        // Core dictionary storage
        [JsonInclude]
        public Dictionary<string, object> Data { get; init; } = new();

        [JsonIgnore]
        public BookNames CurrentBook
        {
            get => Data.TryGetValue("CurrentBook", out var value)
                   ? (BookNames)value : BookNames.Genesis;
            init => Data["CurrentBook"] = value;
        }
        [JsonIgnore]
        public int CurrentChapter
        {
            get => Data.TryGetValue("CurrentChapter", out var value)
                   ? (int)value : 1;
            init => Data["CurrentChapter"] = value;
        }
        [JsonIgnore]
        public int CurrentVerse
        {
            get => Data.TryGetValue("CurrentVerse", out var value)
                   ? (int)value : 1;
            init => Data["CurrentVerse"] = value;
        }
        [JsonIgnore]
        public BookNames NextBook
        {
            get => Data.TryGetValue("NextBook", out var value)
                   ? (BookNames)value : CurrentBook;
            init => Data["NextBook"] = value;
        }
        [JsonIgnore]
        public int NextChapter
        {
            get => Data.TryGetValue("NextChapter", out var value)
                   ? (int)value : CurrentChapter;
            init => Data["NextChapter"] = value;
        }
        [JsonIgnore]
        public int NextVerse
        {
            get => Data.TryGetValue("NextVerse", out var value)
                   ? (int)value : CurrentVerse + 1;
            init => Data["NextVerse"] = value;
        }
        [JsonIgnore]
        public DateTime? LastRead
        {
            get => Data.TryGetValue("LastRead", out var value)
                   ? (DateTime?)value : null;
            init
            {
                if (value.HasValue)
                    Data["LastRead"] = value.Value;
            }
        }

        // Constructor for easy initialization
        public ProgressData()
        {
            // Initialize default values
            CurrentBook = BookNames.Genesis;
            CurrentChapter = 1;
            CurrentVerse = 1;
        }
    }
}