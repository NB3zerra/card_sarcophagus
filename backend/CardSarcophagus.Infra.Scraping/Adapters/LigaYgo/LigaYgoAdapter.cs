using System.Text.Json;
using CardSarcophagus.Infra.Scraping.Interfaces;
using CardSarcophagus.Infra.Scraping.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;

namespace CardSarcophagus.Infra.Scraping.Adapters.LigaYgo
{
    public class LigaYgoAdapter : IPriceProvider
    {
        private readonly IPlaywright _playwright;
        private readonly HttpClient _httpClient;
        private readonly ILogger<LigaYgoAdapter> _logger;
        public LigaYgoAdapter(IHttpClientFactory httpClientFactory, IPlaywright playwright, ILogger<LigaYgoAdapter> logger)
        {
            _httpClient = httpClientFactory.CreateClient();
            _playwright = playwright;
            _logger = logger;
        }

        public async Task<MarketplaceListing> GetPriceAsync(string cardName, string edition, string condition, CancellationToken cancellationToken)
        {
            try
            {
                var url = $"https://www.ligayugioh.com.br/?view=cards/card&card={Uri.EscapeDataString(cardName)}";

                await using var browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
                var page = await browser.NewPageAsync();
                await page.GotoAsync(url);

                var cardInfoContainerHTMLElements = page.Locator("div.container-infos");
                var itemName = (await cardInfoContainerHTMLElements.Locator("div.container-title > div.name > div.item-name").TextContentAsync())?.Trim();
                var englishItemName = (await cardInfoContainerHTMLElements.Locator("div.container-title > div.name > div.item-name-en").TextContentAsync())?.Trim();

                _logger.LogInformation("Starting scraping for card: {cardEnglishName}", englishItemName);

                var showMarketplaceListings = cardInfoContainerHTMLElements.Locator("div.container-show-price-mkp > div.container-price-mkp-item > div.title > span.cursor-pointer");

                //click and wait for the modal show
                await showMarketplaceListings.ClickAsync();
                await page.WaitForSelectorAsync("#full-average-price");

                var marketplaceAveragePricingListingModalHTMLElement = page.Locator("div#full-average-price > div.modal-content > div.body > div#container-fullprice > div.container-edition");

                var tradingCardListing = new List<TradingCardListing>();

                var priceListingCount = await marketplaceAveragePricingListingModalHTMLElement.CountAsync();

                for (var listingIndex = 0; listingIndex < priceListingCount; listingIndex++)
                {
                    var extrasHTMLElement = marketplaceAveragePricingListingModalHTMLElement.Locator("div.edition-price-extras > div.container-extras");

                    var extras = new List<string>();

                    for (var extrasIndex = 0; extrasIndex < await extrasHTMLElement.CountAsync(); extrasIndex++)
                    {
                        var extrasHTMLItem = extrasHTMLElement.Nth(extrasIndex);
                        extras.Add(await extrasHTMLItem.Locator("div").TextContentAsync());
                    }

                    var listingHTMLItem = marketplaceAveragePricingListingModalHTMLElement.Nth(listingIndex);

                    var collectionName = await listingHTMLItem.Locator("div.label-edition > a").TextContentAsync() ?? string.Empty;
                    var cardHigherPrice = (await listingHTMLItem.Locator("div.edition-price-extras > div.price.price-max").TextContentAsync())?.Trim() ?? string.Empty;
                    var cardLowerPrice = (await listingHTMLItem.Locator("div.edition-price-extras > div.price.price-min").TextContentAsync())?.Trim() ?? string.Empty;
                    var cardMidPrice = (await listingHTMLItem.Locator("div.edition-price-extras > div.price.price-medium").TextContentAsync())?.Trim() ?? string.Empty;


                    var listing = new TradingCardListing
                    {
                        CollectionName = collectionName,
                        CardHigherPrice = cardHigherPrice,
                        CardLowerPrice = cardLowerPrice,
                        CardMidPrice = cardMidPrice,
                        CardName = itemName,
                        CardNameInEnglish = englishItemName,
                        Extras = extras

                    };

                    tradingCardListing.Add(listing);
                }

                _logger.LogInformation("Scraping done for card: {cardName}", englishItemName);

                return new MarketplaceListing
                {
                    Marketplace = ESourceMarketplace.LIGA_YGO.ToString(),
                    TradingCardListing = tradingCardListing
                }; ;
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching price");
                return new MarketplaceListing { Marketplace = ESourceMarketplace.LIGA_YGO.ToString() };
            }
        }
    }
}