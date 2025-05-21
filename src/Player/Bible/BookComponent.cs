using System.Text.Json;


namespace Player;

/// <summary>
/// Manages the collection of books that the user has read,
/// including loading from and saving to a JSON save file.
/// </summary>
public class BookComponent : Component<Book>
{

    public Book? CurrentBook { get; private set; }
    private BookProgress CurrentProgess;

    private readonly string path_to_JSON = Path.Combine(
Directory.GetParent(AppContext.BaseDirectory)!.Parent!.Parent!.Parent!.FullName,
"json", "player", "book_component.json");

    /// <summary>
    /// Initializes a new instance of the <see cref="BookComponent"/> class
    /// and loads books from the saved JSON file if it exists.
    /// </summary>
    internal BookComponent() : base("books")
    {
        CurrentBook = null;

    }

    internal void StartNewReading(Bible.BibleBooks bookName, int chapter, int verse)
    {
        // Find book
        if (!FindBook(bookName, out Book? book) || book == null) return;

        // Load in the current Progress of the book from Objects

        // If no progress has been made then create a new progress
        BookProgress newProgress = new(book.Name);
        LogDebug($"Started new Reading in the book of [{bookName}]");

        // Set the current progress
        CurrentProgess = newProgress;

        // Set Current book
        CurrentBook = book;
    }

    private bool FindBook(Bible.BibleBooks bookName, out Book? book)
    {
        Book? foundBook = Objects.Find(item => item.Name == bookName);
        if (foundBook == null)
        {
            LogError($"Cannot find the book of {bookName}");
            book = null;
            return false;
        }
        book = foundBook;
        return true;
    }

    internal void FinishReading(Bible.BibleBooks bookName, int chapter, int verse)
    {
        if (CurrentBook == null || CurrentBook.Name != bookName)
        {
            LogError($"Finished book is not the same as the Currently reading book!!");
            return;
        }
        if (!FindBook(bookName, out Book? book) || book == null) return;

        // Set the new Progress
        CurrentProgess.SetChapter(chapter);
        CurrentProgess.SetVerse(verse);

        // Save data in the book
        // if chapter and verse is at max then go to the next chapter
        // else increment verse
        if (book.ChaptersRead == 0)
            book.IncrementChapters();
        if (book.VersesRead == 0)
            book.IncrementVerses();

        LogDebug($"Finished reading the [{bookName} {chapter}:{verse}]");
    }

    // Track current Bible reading
    public Bible.Book RequestBibleReading()
    {
        // Check Current Book from loaded json to continue where the user left off
        if (CurrentBook is not null)
        {
            LogDebug($"Requested confirmed! Continuing from X");
            return Bible.Book.GetBook(Bible.BibleBooks.Genesis);
        }
        else
        {
            // If null, then start from genesis
            Book firstBook = t_objects[0];

            LogDebug($"Requested confirmed! Starting from the book of {firstBook.Name}");
            return Bible.Book.GetBook(firstBook.Name);
        }
    }

    protected override void LoadLogic()
    {
        base.LoadLogic();
        if (File.Exists(path_to_JSON))
        {
            LogDebug($"Loading BookComponent from JSON.");
            string json = File.ReadAllText(path_to_JSON);
            Book? loadedObject = JsonSerializer.Deserialize<Book>(json);

            if (loadedObject is not null)
            {
                CurrentBook = loadedObject;
            }
        }
        else
        {
            LogInfo($"No saved BookComponent found!");
        }
    }


    protected override void SaveLogic()
    {
        base.SaveLogic();

        // Save this component
        LogDebug($"Saving BookComponent to [{Path.GetFullPath(path_to_JSON)}]");

        // Configure JSON serialization to use string representation for enums
        JsonSerializerOptions? options = new()
        {
            WriteIndented = true,
            Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
        };

        string json = JsonSerializer.Serialize(CurrentProgess, options);
        Directory.CreateDirectory(Path.GetDirectoryName(path_to_JSON)!); // Ensure the directory exists
        File.WriteAllText(path_to_JSON, json);
    }

    private struct BookProgress
    {
        public Bible.BibleBooks Name { get; private set; }
        public int Chapter { get; private set; }
        public int Verse { get; private set; }

        public BookProgress(Bible.BibleBooks Name)
        {
            this.Name = Name;
            Chapter = 0;
            Verse = 0;
        }
        public int[] GetNextReading()
        {
            SetChapter(1);
            SetVerse(1);
            return [Chapter, Verse];
        }

        public void SetChapter(int newChapter) => Chapter = newChapter;
        public void SetVerse(int newVerse) => Verse = newVerse;

    }


}