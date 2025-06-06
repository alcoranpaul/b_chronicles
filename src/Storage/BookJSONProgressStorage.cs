using System.Text.Json;
using System.Text.Json.Serialization;
using Bible;

namespace Player.BibleBook;

public sealed class BookJsonProgressStorage : IBookProgressStorage
{
    private readonly string _filePath;
    private Data _cachedProgress;
    private readonly JsonSerializerOptions _jsonOptions;

    public BookJsonProgressStorage()
    {
        _filePath = Path.Combine(Utils.PathDirHelper.GetPlayerDirectory(), $"bible_progress.json");

        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        };

        _cachedProgress = new Data();
    }

    public void BeginSession(BookNames book, int chapter, int verse)
    {
        Dictionary<BookNames, BookProgressState> bookProgress = _cachedProgress.BookProgress;

        _cachedProgress = new Data
        {
            CurrentBook = book,
            CurrentChapter = chapter,
            CurrentVerse = verse,
            NextBook = book,
            NextChapter = chapter,
            NextVerse = verse + 1,
            BookProgress = bookProgress
        };

        SaveToFile();
    }

    public void SaveProgress(BookNames book, int chapter, int verse,
        (BookNames nextBook, int nextChapter, int nextVerse) next)
    {
        Data newProgress = new()
        {
            CurrentBook = book,
            CurrentChapter = chapter,
            CurrentVerse = verse,
            NextBook = next.nextBook,
            NextChapter = next.nextChapter,
            NextVerse = next.nextVerse,
            LastRead = DateTime.UtcNow,
            BookProgress = _cachedProgress.BookProgress
        };
        _cachedProgress = newProgress;

        SaveToFile();
    }

    // New method to store custom data
    public void StoreCustomData(string key, object value)
    {
        _cachedProgress.RawData[key] = value;
        SaveToFile();
    }

    // New method to retrieve custom data
    public T? GetCustomData<T>(string key)
    {
        if (_cachedProgress.RawData.TryGetValue(key, out var value))
        {
            return (T)value;
        }
        return default;
    }

    public void UpdateBookProgress(BookNames book, BookProgressState state)
    {
        if (!_cachedProgress.BookProgress.ContainsKey(book))
        {

            _cachedProgress.BookProgress.Add(book, state);
        }
        else
        {
            _cachedProgress.BookProgress[book] = state;
        }
        SaveToFile();
    }

    public BookProgressState GetBookProgress(BookNames book)
    {
        return _cachedProgress.BookProgress.TryGetValue(book, out var state)
            ? state
            : BookProgressState.NotStarted;
    }

    private int GetSessionCount()
    {
        return _cachedProgress.RawData.TryGetValue("SessionCount", out var count)
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

    public ProgressData? LoadProgress()
    {
        if (!File.Exists(_filePath)) return null;

        try
        {
            string json = File.ReadAllText(_filePath);
            _cachedProgress = JsonSerializer.Deserialize<Data>(json, _jsonOptions)
                ?? new Data();

            return new(_cachedProgress.CurrentBook, _cachedProgress.CurrentChapter, _cachedProgress.CurrentVerse, _cachedProgress.NextBook, _cachedProgress.NextChapter, _cachedProgress.NextVerse, _cachedProgress.LastRead);
        }
        catch (Exception ex)
        {
            LogError($"Failed to load progress: {ex.Message}");
            return new(BookNames.Genesis, 1, 1); // Safe default
        }
    }

    public void Flush() => SaveToFile();


    private class DictionaryStringObjectJsonConverter : JsonConverter<Dictionary<string, object>>
    {
        public override Dictionary<string, object> Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            Dictionary<string, object>? dictionary = new Dictionary<string, object>();
            using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
            {
                foreach (var property in doc.RootElement.EnumerateObject())
                {
                    dictionary.Add(property.Name, GetValue(property.Value));
                }
            }
            return dictionary;
        }

        private object GetValue(JsonElement element)
        {
            return element.ValueKind switch
            {
                JsonValueKind.String => element.GetString()!,
                JsonValueKind.Number => element.GetInt32(),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                JsonValueKind.Null => null!,
                _ => element.GetRawText() // Fallback for other types
            };
        }

        public override void Write(
            Utf8JsonWriter writer,
            Dictionary<string, object> value,
            JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, options);
        }
    }

    public enum BookProgressState { NotStarted, InProgress, Completed }
    private record Data
    {
        // Core dictionary storage
        [JsonInclude]
        public Dictionary<string, object> RawData { get; init; } = new();

        [JsonInclude]
        public Dictionary<BookNames, BookProgressState> BookProgress { get; init; } = new();

        [JsonIgnore]
        public BookNames CurrentBook
        {
            get => RawData.TryGetValue("CurrentBook", out var value)
                   ? Enum.Parse<BookNames>(value.ToString()!)
                   : BookNames.Genesis;
            init => RawData["CurrentBook"] = value.ToString();
        }

        [JsonIgnore]
        public int CurrentChapter
        {
            get => RawData.TryGetValue("CurrentChapter", out var value)
                   ? int.Parse(value.ToString()!)
                   : 1;
            init => RawData["CurrentChapter"] = value;
        }

        [JsonIgnore]
        public int CurrentVerse
        {
            get => RawData.TryGetValue("CurrentVerse", out var value)
                   ? int.Parse(value.ToString()!)
                   : 1;
            init => RawData["CurrentVerse"] = value;
        }
        [JsonIgnore]
        public BookNames NextBook
        {
            get => RawData.TryGetValue("NextBook", out var value)
                   ? Enum.Parse<BookNames>(value.ToString()!)
                   : BookNames.Genesis;
            init => RawData["NextBook"] = value;
        }
        [JsonIgnore]
        public int NextChapter
        {
            get => RawData.TryGetValue("NextChapter", out var value)
                   ? int.Parse(value.ToString()!)
                   : CurrentChapter + 1;
            init => RawData["NextChapter"] = value;
        }
        [JsonIgnore]
        public int NextVerse
        {
            get => RawData.TryGetValue("NextVerse", out var value)
                   ? int.Parse(value.ToString()!) : CurrentVerse + 1;
            init => RawData["NextVerse"] = value;
        }
        [JsonIgnore]
        public DateTime? LastRead
        {
            get
            {
                if (!RawData.TryGetValue("LastRead", out var value) || value == null)
                    return null;

                if (value is DateTime dt)
                    return dt;

                if (DateTime.TryParse(value.ToString(), out var parsedDate))
                    return parsedDate;

                return null;
            }
            init
            {
                if (value.HasValue)
                    RawData["LastRead"] = value.Value;
                else
                    RawData.Remove("LastRead"); // Or RawData["LastRead"] = null if you want to keep nulls
            }
        }

        // Constructor for easy initialization
        public Data()
        {
            // Initialize default values
            CurrentBook = BookNames.Genesis;
            CurrentChapter = 1;
            CurrentVerse = 1;
        }

        public override string ToString()
        {
            return $"Current Book: {CurrentBook} ({CurrentChapter}:{CurrentVerse}) -> Next: {NextBook} ({NextChapter}:{NextVerse})";
        }
    }


}

public struct ProgressData
{
    public BookNames CurrentBook;
    public int CurrentChapter;
    public int CurrentVerse;
    public BookNames NextBook;
    public int NextChapter;
    public int NextVerse;
    public DateTime? LastRead;

    public ProgressData(BookNames currentBook, int currentChapter, int currentVerse, BookNames nextBook, int nextChapter, int nextVerse, DateTime? lastRead)
    {
        CurrentBook = currentBook;
        CurrentChapter = currentChapter;
        CurrentVerse = currentVerse;
        NextBook = nextBook;
        NextChapter = nextChapter;
        NextVerse = nextVerse;
        LastRead = lastRead;
    }

    public ProgressData(BookNames currentBook, int currentChapter, int currentVerse)
    {
        CurrentBook = currentBook;
        CurrentChapter = currentChapter;
        CurrentVerse = currentVerse;
    }

}