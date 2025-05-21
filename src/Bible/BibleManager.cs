namespace Bible;

public static class BibleManager
{
    public static Dictionary<BibleBooks, Book> Books { get; } =
        Book.GetAllBooks().ToDictionary(book => book.NameAsEnum);

    // Retrieve a book by name (O(1) lookup)
    public static Book? GetBook(BibleBooks bookName)
    {
        Books.TryGetValue(bookName, out Book? book);
        return book;
    }

}