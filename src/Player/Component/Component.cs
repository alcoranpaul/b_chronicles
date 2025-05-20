using System.Text.Json;

namespace Player;

public abstract class Component<T>
{
    protected readonly List<T> t_objects;
    protected readonly string path_to_object;
    protected readonly string object_name;
    public bool IsDefined => t_objects.Count > 0;


    /// <summary>
    /// Initializes a new instance of the <see cref="BookComponent"/> class
    /// and loads books from the saved JSON file if it exists.
    /// </summary>
    public Component(string object_name)
    {
        this.object_name = object_name;
        t_objects = new();
        path_to_object = Path.Combine(
    Directory.GetParent(AppContext.BaseDirectory)!.Parent!.Parent!.Parent!.FullName,
    "json", "player", $"{object_name.ToLower()}.json");
        Load();
    }
    public void End()
    {
        Save();
    }

    /// <summary>
    /// Adds a new object to the user's collection.
    /// </summary>
    /// <param name="object_to_be_added"></param>
    public void AddObject(T object_to_be_added)
    {
        LogInfo($"Adding {object_to_be_added} to user's {object_name} collection.");
        t_objects.Add(object_to_be_added);
    }


    /// <summary>
    /// Saves the current object list to a JSON file on disk.
    /// </summary>
    private void Save()
    {
        try
        {
            LogDebug($"Saving {object_name} to [{Path.GetFullPath(path_to_object)}]: ({t_objects.Count})");

            // Configure JSON serialization to use string representation for enums
            JsonSerializerOptions? options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
            };

            string json = JsonSerializer.Serialize(t_objects, options);
            Directory.CreateDirectory(Path.GetDirectoryName(path_to_object)!); // Ensure the directory exists
            File.WriteAllText(path_to_object, json);
        }
        catch (Exception ex)
        {
            LogError($"Error saving {object_name}: {ex.Message}");
        }
    }

    /// <summary>
    /// Loads the object list from the JSON save file if it exists.
    /// </summary>
    private void Load()
    {
        try
        {
            if (File.Exists(path_to_object))
            {
                LogDebug($"Loading {object_name} from JSON.");
                string json = File.ReadAllText(path_to_object);
                List<T>? loadedObjects = JsonSerializer.Deserialize<List<T>>(json);

                if (loadedObjects != null)
                {
                    t_objects.Clear();
                    t_objects.AddRange(loadedObjects);
                }
            }
            else
            {
                LogInfo($"No saved {object_name} found. Starting fresh.");
            }
        }
        catch (Exception ex)
        {
            LogError($"Error loading {object_name}: {ex.Message}");
        }
    }

}