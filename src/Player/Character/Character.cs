namespace Player;

/// <summary>
/// Represents a character that can be unlocked in the Bible gamification system.
/// Each character has a name, a list of traits, a list of verse references that unlocked them,
/// an description, relationships with other characters, and a count of appearances.
/// </summary>
public class Character
{
    public string Name { get; private set; }
    public List<string> Traits { get; private set; }
    public string? Description { get; set; }
    public List<string>? Relationships { get; set; }
    public int Appearances { get; set; }

    public Character(string name, List<string> traits)
    {
        Name = name;
        Traits = traits;
        Appearances = 1;
        LogDebug($"Initialized Character [{Name}] with initial traits: {string.Join(", ", Traits)}");
    }

    /// <summary>
    /// Add a trait to this character
    /// </summary>
    /// <param name="trait"></param>
    public void AddTrait(string trait)
    {
        if (!Traits.Contains(trait))
        {
            Traits.Add(trait);
            LogDebug($"Added Trait of [{trait}] to {Name}");
        }
    }

    public object ToSerializable()
    {
        return new
        {
            Name,
            Traits,
            Description,
            Relationships,
            Appearances
        };
    }

    public override string ToString()
    {
        return $"{Name}";
    }


}