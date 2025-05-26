

using Bible;

namespace main;

/// <summary>
/// First Time Menu
/// </summary>
public class FTMenu : Menu
{
    public override async Task ShowAsync()
    {
        // TODO: consider to make a user account.
        Options option1 = new Options("Download Bible Raw Data", async () =>
{
    try
    {
        using HttpClient httpClient = new HttpClient();
        var fetcher = new DataFetcher.BibleDataFetcher(httpClient);

        bool success = await fetcher.FetchBibleDataComplete();
        if (success)
        {
            LogDebug("Downloaidng Done!");
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



        await Show("Welcome to Bible Chronicles!", [option1], shouldClearPrev: true, ["The program requires that these dependencies to be installed!"]);
    }
}