
using main;

namespace Player;

public class User
{
    private readonly BookComponent bookComponent;
    private readonly CharacterComponent characterComponent;

    public bool HasBooks => bookComponent.IsDefined;
    public User()
    {
        bookComponent = new();
        characterComponent = new();
        UnlockManager.Instance.OnUnlockedEvent += OnUnlockedEvent;

    }

    private void OnUnlockedEvent(Bible.BibleBooks books, int chapter, int verse, UnlockManager.UnlockEntry entry)
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

    public void AddBibleBook(Bible.BibleBooks book)
    {
        bookComponent.AddObject(new Book(book));
    }

    private void UnlockNewCharacter(BookInfo info, string name, List<string> traits)
    {
        if (!info.IsValid() || string.IsNullOrEmpty(name) || traits == null || traits.Count <= 0) return;

        LogDebug($"Unlocked a new Character of name [{name}]!");
        // If not, add character
        Character newCharacter = new(name, traits);
        characterComponent.AddObject(newCharacter);
    }

    private void AddCharacterTrait(BookInfo info, string name, string value)
    {
        if (!info.IsValid() || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(value)) return;
        Character? existingCharacter = characterComponent.Objects.Find((item) => item.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));

        if (existingCharacter == null) return;


        LogDebug($"Unlocked a new Trait(s) for [{existingCharacter.Name}]!");
        existingCharacter.AddTrait(value);

    }

    public void End()
    {
        bookComponent.End();
        characterComponent.End();
        UnlockManager.Instance.OnUnlockedEvent -= OnUnlockedEvent;
    }

    private struct BookInfo
    {
        public Bible.BibleBooks Book { get; private set; }
        public int Chapter { get; private set; }
        public int Verse { get; private set; }

        public BookInfo(Bible.BibleBooks book, int chapter, int verse)
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