using System.Text.Json;

namespace Bible;

public class Book
{
    public string Name { get; private set; }
    private readonly List<Chapter> chapters;


    public Book(BibleBooks bookName)
    {
        Name = bookName.ToString();
        chapters = new List<Chapter>();

        LogDebug($"Loading book: {Name}");

        string lower_name = Name.ToLower();
        string book_file_path = Path.Combine("json", $"{lower_name}");
        string[] files = Directory.GetFiles(book_file_path, "*.json");
        foreach (string file in files)
        {
            string json = File.ReadAllText(file);

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
            }
            else
            {
                LogError($"Failed to deserialize JSON for book: {Name}");
            }
        }
    }


    public static List<Book> GetAllBooks()
    {
        List<Book> books = new List<Book>();

        foreach (BibleBooks item in Enum.GetValues<BibleBooks>())
        {
            Console.WriteLine(item.ToString());
        }

        return books;
    }

    public static Book GetBook(BibleBooks bookName)
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

    public override string ToString()
    {
        return $"{Name} - {chapters.Count} chapters";
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
