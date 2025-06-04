using System.Text.Json;

namespace Bible;

public class Book
{
    public string Name { get; private set; }
    public BookNames NameAsEnum => Enum.Parse<BookNames>(Name);
    private readonly Chapter[] chapters;




    public Book(BookNames bookName)
    {
        Name = bookName.ToString();

        string convertedBookName = ConvertNameToLower();
        string[] allChapterFiles = ExtractJsonFiles(convertedBookName);
        chapters = new Chapter[allChapterFiles.Length];

        LogDebug($"Loading book: {Name} -- Chapters: {chapters.Length}");

        if (allChapterFiles == null || allChapterFiles.Length <= 0)
        {
            LogCritical($"Missing JSON files for book: {Name}. Consider downloading all books in settings.");
            return;
        }

        ExtractData(allChapterFiles);


    }

    private void ExtractData(string[] allChapterFiles)
    {
        RawBook[] rawBooks = new RawBook[allChapterFiles.Length];

        // Convert to Raw Book
        for (int i = 0; i < allChapterFiles.Length; i++)
        {
            string chapterFile = allChapterFiles[i];
            string json = File.ReadAllText(chapterFile);

            // Deserialize the JSON into the wrapper class
            JsonSerializerOptions? options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            RawBook? wrapper = JsonSerializer.Deserialize<RawBook>(json, options);
            if (wrapper != null && wrapper.Data != null)
                rawBooks[i] = wrapper;
            else
                LogError($"Failed to deserialize JSON for book: {Name}"); ;

        }

        // Conver to Verses and adding into Chapters of this book
        foreach (RawBook chapterFile in rawBooks)
        {
            Verse[] verses = new Verse[chapterFile.Data.Count];
            for (int i = 0; i < chapterFile.Data.Count; i++)
            {
                RawData rawChapter = chapterFile.Data[i];
                Verse verse = new(rawChapter.Text);

                verses[i] = verse;
            }

            int chapterNumber = chapterFile.GetChapter() - 1;

            Chapter chapter = new(verses);
            chapters[chapterNumber] = chapter;

        }
    }

    private static string[] ExtractJsonFiles(string convertedBookName)
    {
        string book_file_dir = Path.Combine("json", "books", $"{convertedBookName}");

        Directory.CreateDirectory(path: book_file_dir);
        string[] allChapterFiles = Directory.GetFiles(book_file_dir, "*.json");
        return allChapterFiles;
    }

    private string ConvertNameToLower()
    {
        string convertedBookName = Name switch
        {
            var name when name.StartsWith("First") => "1" + name.Substring(5),
            var name when name.StartsWith("Second") => "2" + name.Substring(6),
            var name when name.StartsWith("Third") => "3" + name.Substring(5),
            var name when name.StartsWith("Fourth") => "4" + name.Substring(6),
            "SongOfSolomon" => "songofsongs",
            _ => Name
        };
        convertedBookName = convertedBookName.ToLower();
        return convertedBookName;
    }

    public static List<Book> GetAllBooks()
    {
        LogDebug("Get All books");
        List<Book> books = new();
        foreach (BookNames item in Enum.GetValues<BookNames>())
        {
            books.Add(item: new(item));
        }
        return books;
    }

    public static Book GetBook(BookNames bookName)
    {
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
            if (chapter < 1 || chapter > chapters.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(chapter), $"Chapter number is out of range ({chapters.Length}).");
            }

            if (verse < 1 || verse > chapters[chapter - 1].Verses.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(verse), $"Verse number is out of range ({chapters[chapter - 1].Verses.Length}).");
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
        return chapters.Length;
    }

    public int GetVerseCount(int chapterNumber)
    {
        chapterNumber--; // Decrement for 0-indexing
        if (chapterNumber < 0 || chapterNumber > chapters.Length)
        {
            LogDebug($"Requested Verse count for the book of ({Name}). Input: {chapterNumber} -- Chapter Count: {chapters.Length}");
            return -1;
        }

        return chapters[chapterNumber].Verses.Length;
    }

    public override string ToString()
    {
        return $"{Name} - {chapters.Length} chapters";
    }

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

        public override string ToString()
        {
            return Text;
        }
    }

    public class RawData
    {
        public required string Book { get; set; }
        public required string Chapter { get; set; }
        public required string Verse { get; set; }
        public required string Text { get; set; }

        public override string ToString()
        {
            return $"({Book} {Chapter}:{Verse})";
        }

    }

    public class RawBook
    {
        public required List<RawData> Data { get; set; }

        public int GetChapter()
        {
            try
            {
                return int.Parse(Data[0].Chapter);
            }
            catch (ArgumentNullException e)
            {
                LogError($"{e}");
                return -1;
            }
            catch (FormatException e)
            {
                LogError($"{e}");
                return -1;
            }
            catch (OverflowException e)
            {
                LogError($"{e}");
                return -1;
            }

        }

        public override string ToString()
        {
            string msg = "";

            foreach (var item in Data)
            {
                msg += $"{item} ";
            }
            return msg;
        }


    }
}
