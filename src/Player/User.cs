
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
                    UnlockNewCharacter(info, item.Value ?? String.Empty, item.Traits ?? new());
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
        if (!info.IsValid() || String.IsNullOrEmpty(name) || traits == null || traits.Count <= 0) return;

        LogDebug($"Unlcoked Character of name [{name}]!");
        // If not, add character
        Character newCharacter = new(name, traits);
        characterComponent.AddObject(newCharacter);
    }

    public void UnlockCharacter(Bible.BibleBooks book, int chapter, int verse, UnlockManager.Unlock unlockedData)
    {
        // Check if the type is correct.
        if (unlockedData.Type != UnlockManager.UnlockType.Character)
        {
            LogWarning($"UnlockCharacter called with invalid unlock type: {unlockedData.Type}. Expected: Character.");
            return;
        }

        // Validate values 
        CharacterComponent.UnlockCharacter character = new(
    unlockedData.Value ?? string.Empty,
    unlockedData.Traits ?? new()
        );

        if (character.IsNotValid())
        {
            LogWarning($"Unlock Data has discrepancies in its values");
            return;
        }


        Character? existingCharacter = characterComponent.Objects.Find((item) => item.Name.Equals(character.Name, StringComparison.CurrentCultureIgnoreCase));
        if (existingCharacter != null)
        {
            LogDebug($"Adding new trait(s) to {character.Name}");
            // Check if character exists    
            foreach (string trait in character.Traits)
            {
                existingCharacter.AddTrait(trait);
            }
        }
        else
        {
            LogDebug($"Adding {character.Name} Character to user");
            // If not, add character
            Character newCharacter = new(character.Name, character.Traits);
            characterComponent.AddObject(newCharacter);
        }
    }

    public void End()
    {
        bookComponent.End();
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