

using Bible;

namespace main;

public class SettingsMenu : Menu
{
    public override async Task ShowAsync()
    {
        // TODO: consider to make a user account.
        Options option1 = new Options("Download Dependencies", async () =>
        {
            HttpClient? httpClient = new HttpClient();
            DataFetcher.BibleDataFetcher? fetcher = new DataFetcher.BibleDataFetcher(httpClient);

            // Fetch a single verse
            bool success = await fetcher.FetchBibleDataAsync(BookNames.Genesis, 1);
            if (success)
                _stateManager.ChangeState(GameStateManager.State.MainMenu);
        });


        await Show("Settings", shouldClearPrev: true, option1);
    }
}