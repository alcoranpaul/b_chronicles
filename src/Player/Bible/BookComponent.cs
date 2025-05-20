using System.Text.Json;


namespace Player;

/// <summary>
/// Manages the collection of books that the user has read,
/// including loading from and saving to a JSON save file.
/// </summary>
public class BookComponent
{
    private readonly List<Book> books;
    private readonly string PATH_TO_BOOKS = Path.Combine(
        Directory.GetParent(AppContext.BaseDirectory)!.Parent!.Parent!.Parent!.FullName,
        "json", "player", "books.json");

    /// <summary>
    /// Gets a value indicating whether the user currently has any books.
    /// </summary>
    public bool HasBooks => books.Count > 0;


    /// <summary>
    /// Initializes a new instance of the <see cref="BookComponent"/> class
    /// and loads books from the saved JSON file if it exists.
    /// </summary>
    public BookComponent()
    {
        books = new List<Book>();
        LoadBooksFromSave();
    }

    /// <summary>
    /// Saves the current book list to the save file.
    /// </summary>
    public void End()
    {
        SaveBooks();
    }


    /// <summary>
    /// Adds a new book to the user's collection.
    /// </summary>
    /// <param name="book">The <see cref="Bible.BibleBooks"/> book to add.</param>
    public void AddBook(Bible.BibleBooks book)
    {
        LogInfo($"Adding the {Enum.GetName(book)} of Genesis to the user.");
        books.Add(new Book(book));
    }


    /// <summary>
    /// Saves the current list of books to a JSON file on disk.
    /// </summary>
    private void SaveBooks()
    {
        try
        {
            LogDebug($"Saving books to [{Path.GetFullPath(PATH_TO_BOOKS)}]: ({books.Count})");

            // Configure JSON serialization to use string representation for enums
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
            };

            string json = JsonSerializer.Serialize(books, options);
            Directory.CreateDirectory(Path.GetDirectoryName(PATH_TO_BOOKS)!); // Ensure the directory exists
            File.WriteAllText(PATH_TO_BOOKS, json);
        }
        catch (Exception ex)
        {
            LogError($"Error saving books: {ex.Message}");
        }
    }


    /// <summary>
    /// Loads the book list from the JSON save file if it exists.
    /// </summary>
    private void LoadBooksFromSave()
    {
        try
        {
            if (File.Exists(PATH_TO_BOOKS))
            {
                LogDebug("Loading books");
                string json = File.ReadAllText(PATH_TO_BOOKS);
                List<Book>? loadedBooks = JsonSerializer.Deserialize<List<Book>>(json);

                if (loadedBooks != null)
                {
                    books.Clear();
                    books.AddRange(loadedBooks);
                }
            }
            else
            {
                LogInfo("No saved books found. Starting fresh.");
            }
        }
        catch (Exception ex)
        {
            LogError($"Error loading books: {ex.Message}");
        }
    }
}