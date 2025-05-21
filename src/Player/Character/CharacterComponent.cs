namespace Player;

/// <summary>
/// Manages a collection of unlocked characters for a user,
/// including adding, retrieving, and updating character-related information.
/// </summary>
public class CharacterComponent : Component<Character>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CharacterComponent"/> class
    /// and loads books from the saved JSON file if it exists.
    /// </summary>
    public CharacterComponent() : base("character")
    {

    }

    public struct UnlockCharacter
    {
        public UnlockCharacter(string name, List<string> traits)
        {
            Name = name;
            Traits = traits;
        }

        public string Name { get; private set; }
        public List<string> Traits { get; private set; }

        public readonly bool IsNotValid()
        {
            bool isNameNotEmpty = Name == string.Empty;
            bool isTraitsValid = Traits != null && Traits.Any(string.IsNullOrEmpty);

            return isNameNotEmpty || isTraitsValid;
        }


    }
}