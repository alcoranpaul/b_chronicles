

namespace Player.BibleBook;

public class Book : IEquatable<Book>
{
    public Bible.BookNames Name { get; private set; }
    public int ChaptersRead { get; private set; }
    public int VersesRead { get; private set; }

    public Book(Bible.BookNames name)
    {
        Name = name;
        ChaptersRead = 0;
        VersesRead = 0;
    }

    public void IncrementChapters()
    {
        // Get the raw book
        // Bible.Book? rawBook = Bible.BibleManager.GetBook(Name);
        // if (rawBook == null)
        // {
        //     LogError($"Cannot find Raw Data of the book of {Name}");
        //     return;
        // }
        // // Figure out if the verse is at max
        // int versesCount = rawBook.GetVerseCount(ChaptersRead);
        // if (versesCount == VersesRead)
        //     ChaptersRead++;
        // else
        // {
        //     LogError($"Attempted to increase chapters when verse is not completed");
        // }
        ChaptersRead++;
    }
    public void DecrementChapters() => ChaptersRead--;
    public void IncrementVerses() => VersesRead++;
    public void DecrementVerses() => VersesRead--;

    // IEquatable implementation
    public bool Equals(Book? other)
    {
        // Null check (including for 'this' if called via object.Equals)
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return Name == other.Name &&
               ChaptersRead == other.ChaptersRead &&
               VersesRead == other.VersesRead;

    }
    // Override Object.Equals
    public override bool Equals(object? obj)
    {
        return obj is Book other && Equals(other);
    }

    // Override GetHashCode
    public override int GetHashCode()
    {
        return HashCode.Combine(Name, ChaptersRead, VersesRead);
    }

    // Operator overloads
    public static bool operator ==(Book? left, Book? right) => left is not null && right is not null && left.Equals(right);
    public static bool operator !=(Book? left, Book? right) => !(left == right);

    // Comparison method for sorting/ordering
    public static int CompareByProgress(Book x, Book y)
    {
        int nameComparison = x.Name.CompareTo(y.Name);
        if (nameComparison != 0) return nameComparison;

        int chapterComparison = x.ChaptersRead.CompareTo(y.ChaptersRead);
        if (chapterComparison != 0) return chapterComparison;

        return x.VersesRead.CompareTo(y.VersesRead);
    }
}