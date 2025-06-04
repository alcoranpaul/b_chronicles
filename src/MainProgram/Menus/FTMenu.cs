

using Bible;

namespace main;

/// <summary>
/// First Time Menu
/// </summary>
public class FTMenu : Menu
{
    /// <summary>
    /// Displays a menu with two options to download the Bible data.
    /// </summary>
    /// <remarks>
    /// The first option downloads the complete Bible raw data. This process takes around 30 minutes.
    /// The second option downloads only the Genesis book.
    /// </remarks>
    /// <returns>A Task representing the asynchronous menu operation.</returns>
    public override async Task ShowAsync()
    {
        // TODO: consider to make a user account.
        Options option1 = new Options("Download Complete Bible Raw Data (~30 mins)", async () =>
            {
                try
                {
                    using HttpClient httpClient = new HttpClient();
                    var fetcher = new DataFetcher.BibleDataFetcher(httpClient);
                    LogInfo($"Downloading Bible data...");
                    bool success = await fetcher.FetchBibleDataComplete();
                    if (success)
                    {
                        LogDebug("Downloading Done!");
                        _stateManager.ChangeState(GameStateManager.State.MainMenu);
                    }
                    else
                    {
                        LogError("Failed to download Bible data.");
                    }
                }
                catch (Exception ex)
                {
                    LogError($"Exception during Bible download: {ex}");
                }
            });

        Options option2 = new Options("Download Genesis", async () =>
       {
           try
           {
               using HttpClient httpClient = new HttpClient();
               var fetcher = new DataFetcher.BibleDataFetcher(httpClient);
               LogInfo($"Downloading Genesis...");
               bool success = await fetcher.DownloadBook(BookNames.Genesis);
               if (success)
               {
                   LogDebug("Downloading Done!");
                   _stateManager.ChangeState(GameStateManager.State.MainMenu);
               }
               else
               {
                   LogError("Failed to download Bible Genesis.");
               }
           }
           catch (Exception ex)
           {
               LogError($"Exception during Genesis download: {ex}");
           }
       });
        Options exitOption = new("Exit", () =>
       {
           LogDebug("Requested to end application");
           _stateManager.ChangeState(GameStateManager.State.End);
       });



        await Show("Welcome to Bible Chronicles!", [option1, exitOption], shouldClearPrev: true, ["The program requires that these dependencies to be installed!"]);
    }
}