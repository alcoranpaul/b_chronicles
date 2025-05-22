
using main;
using Player.BibleBook;

namespace Player;

public class User
{
    public static User Instance { get; private set; } = new User();
    private readonly BookTracker _bookTracker;

    private readonly CharacterComponent _characterComponent;
    public List<Character> Characters => _characterComponent.Objects;


    private User()
    {
        _bookTracker = new();
        _characterComponent = new();

        UnlockManager.Instance.OnUnlockedEvent += OnUnlockedEvent;
        TypingSessionManager.OnSessionCompleted += OnVerseCompleted;
        TypingSessionManager.OnSessionStarted += OnStartReadingVerse;

    }

    private void OnStartReadingVerse(Bible.BookNames bookName, int chapter, int verse)
    {
        _bookTracker.StartReading(bookName, chapter, verse);
    }

    private void OnVerseCompleted(Bible.BookNames bookName, int chapter, int verse)
    {
        _bookTracker.FinishReading(bookName, chapter, verse);
    }

    private void OnUnlockedEvent(Bible.BookNames books, int chapter, int verse, UnlockManager.UnlockEntry entry)
    {
        if (entry == null || entry.Unlocks == null || entry.Unlocks.Count <= 0) return;

        BookInfo info = new(books, chapter, verse);
        foreach (UnlockManager.Unlock item in entry.Unlocks)
        {
            switch (item.Type)
            {
                case UnlockManager.UnlockType.Character:
                    UnlockNewCharacter(info, item.Value ?? string.Empty, item.Traits ?? new());
                    continue;
                case UnlockManager.UnlockType.Trait:
                    AddCharacterTrait(info, item.Character ?? string.Empty, item.Value ?? string.Empty);
                    continue;
                default:
                    continue;
            }
        }
    }

    private void UnlockNewCharacter(BookInfo info, string name, List<string> traits)
    {
        if (!info.IsValid() || string.IsNullOrEmpty(name) || traits == null || traits.Count <= 0) return;

        LogDebug($"Unlocked a new Character of name [{name}]!");
        // If not, add character
        Character newCharacter = new(name, traits);
        _characterComponent.AddObject(newCharacter);
    }

    private void AddCharacterTrait(BookInfo info, string name, string value)
    {
        if (!info.IsValid() || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(value)) return;
        Character? existingCharacter = _characterComponent.Objects.Find((item) => item.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));

        if (existingCharacter == null) return;


        LogDebug($"Unlocked a new Trait(s) for [{existingCharacter.Name}]!");
        existingCharacter.AddTrait(value);

    }

    public void End()
    {
        _bookTracker.Dispose();
        _characterComponent.End();
        UnlockManager.Instance.OnUnlockedEvent -= OnUnlockedEvent;
    }

    public (Bible.BookNames book, int chapter, int verse) RequestBibleReading() => _bookTracker.GetNextReading();

    private struct BookInfo
    {
        public Bible.BookNames Book { get; private set; }
        public int Chapter { get; private set; }
        public int Verse { get; private set; }

        public BookInfo(Bible.BookNames book, int chapter, int verse)
        {
            Book = book;
            Chapter = chapter;
            Verse = verse;
        }

        public readonly bool IsValid()
        {
            return Chapter > 0 && Verse > 0;
        }

    }

}