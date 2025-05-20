using System.Text.Json;


namespace Player;

public class BookComponent
{
    private readonly List<Book> books = new List<Book>();
    private readonly string PATH_TO_BOOKS = Path.Combine(
        Directory.GetParent(AppContext.BaseDirectory)!.Parent!.Parent!.Parent!.FullName,
        "json", "player", "books.json");
    public bool HasBooks => books.Count > 0;

    public BookComponent()
    {
        LoadBooksFromSave();
    }

    public void End()
    {
        SaveBooks();
    }

    public void AddBook(Bible.BibleBooks book)
    {
        LogInfo($"Adding the {Enum.GetName(book)} of Genesis to the user.");
        books.Add(new Book(book));
    }

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