using Microsoft.Playwright;

namespace CardSarcophagus.Infra.Scraping.Adapters.Base
{
    public class BaseScrapingAdapter
    {
        private readonly IPlaywright _playwright;
        private readonly IBrowser _browser;

        public BaseScrapingAdapter(
            IPlaywright playwright,
            IBrowser browser
        )
        {
            _playwright = playwright;
            _browser = browser;
        }

        protected async Task<IPage> NavigateToPage(string url)
        {
            var page = await _browser.NewPageAsync();
            await page.GotoAsync(url);

            return page;
        } 

    }
}