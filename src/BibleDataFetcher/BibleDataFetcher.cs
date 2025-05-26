
using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace DataFetcher;

public class BibleDataFetcher
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl = "https://cdn.jsdelivr.net/gh/wldeh/bible-api/bibles/en-lsv/books";

    public BibleDataFetcher(HttpClient httpClient)
    {
        _httpClient = httpClient;

    }

    public async Task<bool> FetchBibleDataAsync(Bible.BookNames book, int chapter, int? verse = null)
    {
        try
        {

            string bookName = book.ToString().ToLower();
            // Build the API URL
            string url = verse.HasValue
                ? $"{_baseUrl}/{bookName}/chapters/{chapter}/verses/{verse}.json"
                : $"{_baseUrl}/{bookName}/chapters/{chapter}.json";
            LogDebug($"Downloading JSON file from {url}");

            // Fetch the data
            using var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"❌ Error: {response.StatusCode}");
                return false;
            }

            // Parse and clean the JSON
            var jsonData = await response.Content.ReadAsStringAsync();
            using JsonDocument doc = JsonDocument.Parse(jsonData);
            var root = doc.RootElement;

            var cleanedData = CleanJsonData(root);

            // Save to file
            string outputDir = Path.Combine("json", bookName);
            Directory.CreateDirectory(outputDir);

            string fileName = verse.HasValue
                ? $"{bookName}_chapter_{chapter}_verse_{verse}.json"
                : $"{bookName}_chapter_{chapter}.json";

            string outputFile = Path.Combine(outputDir, fileName);

            await using (FileStream fs = File.Create(outputFile))
            {
                await JsonSerializer.SerializeAsync(fs, cleanedData, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });
            }

            LogDebug($"📁 JSON data written to '{outputFile}'");
            return true;
        }
        catch (Exception ex)
        {
            LogError($"❌ Exception: {ex.Message}");
            return false;
        }
    }

    private JsonElement CleanJsonData(JsonElement data)
    {
        using var ms = new MemoryStream();
        using (var writer = new Utf8JsonWriter(ms))
        {
            writer.WriteStartObject(); // Start root object
            writer.WritePropertyName("data");
            writer.WriteStartArray(); // Start data array

            // Handle array response (multiple verses)
            if (data.TryGetProperty("data", out var dataArray) && dataArray.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in dataArray.EnumerateArray())
                {
                    WriteCleanedVerse(writer, item);
                }
            }
            // Handle single verse response
            else if (data.ValueKind == JsonValueKind.Object && data.TryGetProperty("text", out _))
            {
                WriteCleanedVerse(writer, data);
            }
            // Handle unexpected format
            else
            {
                writer.WriteStartObject();
                writer.WriteString("error", "Unexpected data format");
                writer.WriteEndObject();
            }

            writer.WriteEndArray(); // End data array
            writer.WriteEndObject(); // End root object
        }

        ms.Position = 0;
        var cleanedDoc = JsonDocument.Parse(ms);
        return cleanedDoc.RootElement.Clone();
    }

    private void WriteCleanedVerse(Utf8JsonWriter writer, JsonElement verse)
    {
        writer.WriteStartObject();
        foreach (var prop in verse.EnumerateObject())
        {
            if (prop.Name == "text" && prop.Value.ValueKind == JsonValueKind.String)
            {
                string cleanedText = prop.Value.GetString()?
                    .Replace("[", "")
                    .Replace("]", "") ?? string.Empty;
                writer.WriteString("text", cleanedText);
            }
            else
            {
                // Preserve all other properties exactly
                prop.WriteTo(writer);
            }
        }
        writer.WriteEndObject();
    }
}

