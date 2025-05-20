
using Bible;
namespace Player;

public struct Book
{
    public BibleBooks Name { get; private set; }
    public int ChaptersRead { get; private set; }
    public int VersesRead { get; private set; }

    public Book(BibleBooks name)
    {
        Name = name;
        ChaptersRead = 0;
        VersesRead = 0;
    }

    public void IncrementChapters() => ChaptersRead++;
    public void IncrementVerses() => VersesRead++;


}