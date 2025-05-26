using System.Text.Json;

namespace Bible;

public class Book
{
    public string Name { get; private set; }
    public BookNames NameAsEnum => Enum.Parse<BookNames>(Name);
    private readonly List<Chapter> chapters;




    public Book(BookNames bookName)
    {
        Name = bookName.ToString();
        chapters = new List<Chapter>();

        LogDebug($"Loading book: {Name}");

        string lower_name = Name.ToLower();
        string book_file_dir = Path.Combine("json", $"{lower_name}");

        Directory.CreateDirectory(book_file_dir);
        string[] allChapterFiles = Directory.GetFiles(book_file_dir, "*.json");

        if (allChapterFiles == null || allChapterFiles.Length <= 0)
        {
            LogError($"No files in direcotyr");
            return;
        }

        int index = 0;
        foreach (string chapterFile in allChapterFiles)
        {
            index++;
            string json = File.ReadAllText(chapterFile);

            // Deserialize the JSON into the wrapper class
            JsonSerializerOptions? options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            RawBook? wrapper = JsonSerializer.Deserialize<RawBook>(json, options);

            if (wrapper != null && wrapper.Data != null)
            {
                Verse[] verses = new Verse[wrapper.Data.Count];
                for (int i = 0; i < wrapper.Data.Count; i++)
                {
                    RawData rawChapter = wrapper.Data[i];
                    Verse verse = new Verse(rawChapter.Text);
                    verses[i] = verse;
                }

                Chapter chapter = new Chapter(verses);
                chapters.Add(chapter);
                LogDebug($"Adding Chapter {index} for book [{Name}]");
            }
            else
            {
                LogError($"Failed to deserialize JSON for book: {Name}");
            }
        }
    }


    public static List<Book> GetAllBooks()
    {
        LogDebug("Get All books");
        List<Book> books = [new(BookNames.Genesis)];
        // foreach (BookNames item in Enum.GetValues<BookNames>())
        //     books.Add(item: new(item));
        return books;
    }

    public static Book GetBook(BookNames bookName)
    {
        LogDebug("Get books");
        return new Book(bookName);
    }

    /// <summary>
    /// Gets the number of chapters in the book.
    /// </summary>
    /// <param name="chapter"></param>
    /// <param name="verse"></param>
    /// <returns></returns>
    public string GetVerse(int chapter, int verse)
    {
        try
        {
            if (chapter < 1 || chapter > chapters.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(chapter), "Chapter number is out of range.");
            }

            if (verse < 1 || verse > chapters[chapter - 1].Verses.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(verse), "Verse number is out of range.");
            }
        }
        catch (ArgumentOutOfRangeException ex)
        {
            LogError(ex.Message);
            return string.Empty;
        }

        return chapters[chapter - 1].Verses[verse - 1].Text;
    }

    public int GetChaptersCount()
    {
        return chapters.Count;
    }

    public int GetVerseCount(int chapterNumber)
    {
        chapterNumber--; // Decrement for 0-indexing
        if (chapterNumber < 0 || chapterNumber > chapters.Count)
        {
            LogDebug($"Requested Verse count for the book of ({Name}). Input: {chapterNumber} -- Chapter Count: {chapters.Count}");
            return -1;
        }

        return chapters[chapterNumber].Verses.Length;
    }

    public override string ToString()
    {
        return $"{Name} - {chapters.Count} chapters";
    }

    // Override Equals and GetHashCode to compare by Name
    public override bool Equals(object? obj)
    {
        return obj is Book other && Name == other.Name;
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }




    private class Chapter
    {
        public Verse[] Verses { get; private set; }

        public Chapter(Verse[] Verses)
        {
            this.Verses = Verses;
        }


    }

    private class Verse
    {
        public string Text { get; private set; }

        public Verse(string text)
        {
            Text = text;
        }
    }

    public class RawData
    {
        public required string Book { get; set; }
        public required string Chapter { get; set; }
        public required string Verse { get; set; }
        public required string Text { get; set; }
    }

    public class RawBook
    {
        public required List<RawData> Data { get; set; }
    }
}
