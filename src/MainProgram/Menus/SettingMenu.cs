

using Bible;

namespace main;

public class SettingsMenu : Menu
{
    public override async Task ShowAsync()
    {
        // TODO: consider to make a user account.
        Options option1 = new Options("Application Information", () =>
        {
            _stateManager.ChangeState(GameStateManager.State.AppInfo);
        });

        Options option2 = new Options("Add to Windows Scheduler", async () =>
        {
            SchedulerMenu schedulerMenu = new();
            await schedulerMenu.ShowAsync();
        });

        Options option3 = new Options("Download all Bible books", async () =>
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


        Options exitOption = new("Back to Main Menu", () =>
       {
           _stateManager.ChangeState(GameStateManager.State.MainMenu);
       });


        if (Utils.PlatformHelper.IsWindows())
            await Show("Settings", shouldClearPrev: true, option1, option2, option3, exitOption);
        else
            await Show("Settings", shouldClearPrev: true, option1, option3, exitOption);
    }
}