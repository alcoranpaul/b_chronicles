using System.Text.Json;


namespace Player;

/// <summary>
/// Manages the collection of books that the user has read,
/// including loading from and saving to a JSON save file.
/// </summary>
public class BookComponent : Component<Book>
{

    /// <summary>
    /// Initializes a new instance of the <see cref="BookComponent"/> class
    /// and loads books from the saved JSON file if it exists.
    /// </summary>
    public BookComponent() : base("Books")
    {
    }


}