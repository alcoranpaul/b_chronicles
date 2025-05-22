

namespace Player.BibleBook;

/// <summary>
/// Provides metadata and navigation services for Bible books, chapters, and verses.
/// </summary>
/// <remarks>
/// This class maintains efficient lookups for Bible structure data and validates scripture references.
/// </remarks>
public sealed class BookMetadata
{
    private readonly Dictionary<Bible.BookNames, Bible.Book> _books;
    private readonly Dictionary<Bible.BookNames, int> _chapterCounts;
    private readonly Dictionary<(Bible.BookNames book, int chapter), int> _verseCounts;

    /// <summary>
    /// Initializes a new instance of the <see cref="BookMetadata"/> class.
    /// </summary>
    /// <remarks>
    /// Loads all Bible books and precomputes chapter/verse counts for efficient lookups.
    /// </remarks>
    public BookMetadata()
    {
        _books = Bible.Book.GetAllBooks().ToDictionary(book => book.NameAsEnum);
        LogDebug($"{_books}");
        // Initialize chapter counts
        _chapterCounts = new();
        foreach (var book in _books.Values)
        {
            _chapterCounts[book.NameAsEnum] = book.GetChaptersCount();
        }

        // Initialize verse counts
        _verseCounts = new();
        foreach (KeyValuePair<Bible.BookNames, Bible.Book> bookPair in _books)
        {
            var book = bookPair.Value;
            for (int chapter = 1; chapter <= book.GetChaptersCount(); chapter++)
            {
                _verseCounts[(book.NameAsEnum, chapter)] = book.GetVerseCount(chapter);
            }
        }
    }

    /// <summary>
    /// Gets the next book in canonical order.
    /// </summary>
    /// <param name="currentBook">The current book.</param>
    /// <returns>
    /// The next book in the Bible, or null if <paramref name="currentBook"/> is Revelation.
    /// </returns>
    /// <example>
    /// <code>
    /// var next = metadata.GetNextBook(Bible.BookNames.Genesis); // Returns Exodus
    /// </code>
    /// </example>
    public bool GetNextBookName(Bible.BookNames currentBook, out Bible.BookNames nextBook)
    {
        Bible.BookNames[] allBooks = Enum.GetValues<Bible.BookNames>();
        int currentIndex = Array.IndexOf(allBooks, currentBook);
        int nextIndex = currentIndex + 1;
        if (nextIndex < allBooks.Length)
        {
            nextBook = allBooks[nextIndex];
            return true;
        }
        nextBook = currentBook;
        return false;
    }

    /// <summary>
    /// Validates whether a specific Bible passage exists.
    /// </summary>
    /// <param name="bookName">The book of the Bible.</param>
    /// <param name="chapter">The chapter number (1-based).</param>
    /// <param name="verse">The verse number (1-based).</param>
    /// <returns>
    /// True if the passage exists; otherwise, false.
    /// </returns>
    /// <example>
    /// <code>
    /// bool valid = metadata.IsValidPassage(Bible.BookNames.Psalms, 119, 176); // Returns true
    /// bool invalid = metadata.IsValidPassage(Bible.BookNames.Psalms, 150, 7); // Returns false
    /// </code>
    /// </example>
    public bool IsValidPassage(Bible.BookNames bookName, int chapter, int verse)
    {
        if (!_books.TryGetValue(bookName, out Bible.Book? bookObj))
        {
            LogDebug($"Cannot retrieve {bookName} {chapter}:{verse}");
            return false;
        }

        bool isChapterVerseValid = chapter >= 1 &&
               chapter <= bookObj.GetChaptersCount() &&
               verse >= 1 &&
               verse <= bookObj.GetVerseCount(chapter);

        LogDebug($"Is Chapter and Verse valid? {isChapterVerseValid}");

        return isChapterVerseValid;
    }

    /// <summary>
    /// Gets the number of verses in a specific chapter.
    /// </summary>
    /// <param name="bookName">The book of the Bible.</param>
    /// <param name="chapter">The chapter number (1-based).</param>
    /// <returns>The number of verses in the specified chapter.</returns>
    /// <exception cref="KeyNotFoundException">
    /// Thrown when the book/chapter combination does not exist.
    /// </exception>
    public int GetVerseCount(Bible.BookNames bookName, int chapter) => _verseCounts[(bookName, chapter)];

    /// <summary>
    /// Gets the number of chapters in a specific book.
    /// </summary>
    /// <param name="bookName">The book of the Bible.</param>
    /// <returns>The number of chapters in the specified book.</returns>
    /// <exception cref="KeyNotFoundException">
    /// Thrown when the book does not exist.
    /// </exception>
    public int GetChapterCount(Bible.BookNames bookName) => _chapterCounts[bookName];
}