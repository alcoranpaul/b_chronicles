
using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Bible;

namespace DataFetcher;

public class BibleDataFetcher
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl = "https://cdn.jsdelivr.net/gh/wldeh/bible-api/bibles/en-lsv/books";

    public BibleDataFetcher(HttpClient httpClient)
    {
        _httpClient = httpClient;

    }

    public async Task<bool> FetchBibleDataComplete()
    {
        bool overallSuccess = true;
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
        httpClient.Timeout = TimeSpan.FromSeconds(60);

        var fetcher = new BibleDataFetcher(httpClient);

        BookNames[] allBooks = (BookNames[])Enum.GetValues(typeof(BookNames));
        int totalBooks = allBooks.Length;
        int completedBooks = 0;

        LogDebug($"Starting download of {totalBooks} books");

        using var cts = new CancellationTokenSource();
        var spinnerTask = ShowLiveSpinnerAsync("Downloading Bible", cts.Token); // Start spinner

        foreach (BookNames book in allBooks)
        {
            int chapter = 1;
            bool bookComplete = false;
            int consecutiveFailures = 0;
            const int maxFailures = 3;

            completedBooks++;
            LogDebug($"Starting {book} (book {completedBooks}/{totalBooks})");

            while (!bookComplete)
            {
                try
                {
                    RenderProgressBar(completedBooks, totalBooks, book.ToString(), chapter);

                    bool success = await fetcher.FetchBibleDataAsync(book, chapter).ConfigureAwait(false);

                    if (success)
                    {
                        RenderProgressBar(completedBooks, totalBooks, book.ToString(), chapter, success: true);

                        LogDebug($"Downloading Sucess for {book} {chapter}");
                        consecutiveFailures = 0;
                        chapter++;

                        await Task.Delay(200).ConfigureAwait(false);
                    }
                    else
                    {

                        consecutiveFailures++;
                        LogDebug($"Failed {book} chapter {chapter} (attempt {consecutiveFailures}/{maxFailures})");

                        if (consecutiveFailures >= maxFailures)
                        {
                            if (chapter > 1)
                            {
                                LogDebug($"Assuming end of {book} at chapter {chapter - 1}");
                                bookComplete = true;
                            }
                            else
                            {
                                LogDebug($"Aborting {book} - no chapters downloaded");
                                overallSuccess = false;
                                bookComplete = true;
                            }
                        }
                        else
                        {
                            await Task.Delay(500).ConfigureAwait(false);
                        }
                    }
                }
                catch (HttpRequestException httpEx)
                {
                    LogDebug($"HTTP error downloading {book} chapter {chapter}: {httpEx.Message}");
                    consecutiveFailures++;

                    if (consecutiveFailures >= maxFailures)
                    {
                        RenderProgressBar(completedBooks, totalBooks, book.ToString(), chapter, success: false, status: "HttpRequestException");

                        overallSuccess = false;
                        bookComplete = true;
                    }
                    else
                    {
                        await Task.Delay(1000).ConfigureAwait(false);
                    }
                }
                catch (TaskCanceledException)
                {
                    LogDebug($"Timeout downloading {book} chapter {chapter}");
                    consecutiveFailures++;

                    if (consecutiveFailures >= maxFailures)
                    {
                        RenderProgressBar(completedBooks, totalBooks, book.ToString(), chapter, success: false, status: "TaskCanceledException");
                        overallSuccess = false;
                        bookComplete = true;
                    }
                    else
                    {
                        await Task.Delay(1000).ConfigureAwait(false);
                    }
                }
                catch (Exception ex)
                {
                    RenderProgressBar(completedBooks, totalBooks, book.ToString(), chapter, success: false, status: "Exception");
                    LogDebug($"Unexpected error downloading {book} chapter {chapter}: {ex}");
                    overallSuccess = false;
                    bookComplete = true;
                }
            }

            await Task.Delay(1000).ConfigureAwait(false); // Delay between books
        }

        cts.Cancel();  // Stop the spinner
        await spinnerTask;  // Ensure spinner finishes

        Console.WriteLine($"\nDownload {(overallSuccess ? "completed" : "partially completed")}");
        return overallSuccess;
    }

    public async Task<bool> DownloadBook(BookNames book)
    {
        using var httpClient = new HttpClient();
        var fetcher = new BibleDataFetcher(httpClient);

        int chapter = 1;
        bool bookComplete = false;
        int consecutiveFailures = 0;
        const int maxFailures = 3;

        while (!bookComplete)
        {
            try
            {
                bool success = await fetcher.FetchBibleDataAsync(book, chapter).ConfigureAwait(false);

                if (success)
                {
                    Console.WriteLine($"Downloaded chapter {chapter} of {book}");
                    consecutiveFailures = 0;
                    chapter++;

                    await Task.Delay(200).ConfigureAwait(false);
                }
                else
                {
                    consecutiveFailures++;
                    Console.WriteLine($"Failed to download chapter {chapter} of {book} (attempt {consecutiveFailures}/{maxFailures})");

                    if (consecutiveFailures >= maxFailures)
                    {
                        if (chapter > 1)
                        {
                            Console.WriteLine($"Assuming end of {book} at chapter {chapter - 1}");
                            bookComplete = true;
                        }
                        else
                        {
                            Console.WriteLine($"Aborting {book} - no chapters downloaded");
                            bookComplete = true;
                        }
                    }
                    else
                    {
                        await Task.Delay(500).ConfigureAwait(false);
                    }
                }
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"HTTP error downloading chapter {chapter} of {book}: {httpEx.Message}");
                consecutiveFailures++;

                if (consecutiveFailures >= maxFailures)
                {
                    Console.WriteLine($"Aborting {book} - too many HTTP errors");
                    bookComplete = true;
                }
                else
                {
                    await Task.Delay(1000).ConfigureAwait(false);
                }
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine($"Timeout downloading chapter {chapter} of {book}");
                consecutiveFailures++;

                if (consecutiveFailures >= maxFailures)
                {
                    Console.WriteLine($"Aborting {book} - too many timeouts");
                    bookComplete = true;
                }
                else
                {
                    await Task.Delay(1000).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error downloading chapter {chapter} of {book}: {ex}");
                bookComplete = true;
            }
        }

        return bookComplete && chapter > 1;
    }

    public async Task<bool> FetchBibleDataAsync(BookNames book, int chapter, int? verse = null)
    {
        try
        {

            string bookName = book.ToString();
            bookName = bookName switch
            {
                var name when name.StartsWith("First") => "1" + name.Substring(5),
                var name when name.StartsWith("Second") => "2" + name.Substring(6),
                var name when name.StartsWith("Third") => "3" + name.Substring(5),
                var name when name.StartsWith("Fourth") => "4" + name.Substring(6),
                "SongOfSolomon" => "songofsongs",
                _ => bookName
            };
            bookName = bookName.ToLower();

            // Build the API URL
            string url = verse.HasValue
                ? $"{_baseUrl}/{bookName}/chapters/{chapter}/verses/{verse}.json"
                : $"{_baseUrl}/{bookName}/chapters/{chapter}.json";

            // Fetch the data
            LogDebug($"Downloading JSON file from {url}");
            using var response = await _httpClient.GetAsync(url); // suspect line
            LogDebug($"Received response for {book} chapter {chapter}");

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            // Parse and clean the JSON
            var jsonData = await response.Content.ReadAsStringAsync();
            using JsonDocument doc = JsonDocument.Parse(jsonData);
            var root = doc.RootElement;

            var cleanedData = CleanJsonData(root);

            // Save to file
            string outputDir = Path.Combine(Utils.PathDirHelper.GetBooksDirectory(), bookName);
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
    private async Task ShowLiveSpinnerAsync(string message, CancellationToken token)
    {
        Console.Write(message + " ");
        char[] spinner = ['|', '/', '-', '\\'];
        int index = 0;

        while (!token.IsCancellationRequested)
        {
            Console.Write(spinner[index++ % spinner.Length]);
            await Task.Delay(100);
            Console.Write("\b");
        }

        Console.WriteLine("✔"); // Show check mark when done
    }

    private void RenderProgressBar(int current, int total, string book, int chapter, bool? success = null, string status = "")
    {
        int barWidth = 20;
        double percent = (double)current / total;
        int filled = (int)(percent * barWidth);
        string progressBar = "[" + new string('=', filled) + new string(' ', barWidth - filled) + "]";
        string state = success.HasValue
            ? (success.Value ? "✅ Success" : $"❌ {status}")
            : "";

        string message = $"{progressBar}  {current}/{total}  {book,-12} {chapter,-3}  {state}";
        Console.Write("\r" + message.PadRight(Console.WindowWidth - 1));
    }


}

