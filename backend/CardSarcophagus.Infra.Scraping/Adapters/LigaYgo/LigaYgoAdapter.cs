using System.Text.Json;
using CardSarcophagus.Infra.Scraping.Adapters.Base;
using CardSarcophagus.Infra.Scraping.Interfaces;
using CardSarcophagus.Infra.Scraping.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;

namespace CardSarcophagus.Infra.Scraping.Adapters.LigaYgo
{
    public class LigaYgoAdapter : BaseScrapingAdapter, IPriceProvider
    {
        private readonly ILogger<LigaYgoAdapter> _logger;
        public LigaYgoAdapter(
            IPlaywright playwright,
            IBrowser browser,
            ILogger<LigaYgoAdapter> logger
            )
        : base(playwright, browser)
        {
            _logger = logger;
        }

        public async Task<MarketplaceListing> GetPriceAsync(string cardName, string edition, string condition, CancellationToken cancellationToken)
        {
            try
            {
                var page = await NavigateToPage($"https://www.ligayugioh.com.br/?view=cards/card&card={Uri.EscapeDataString(cardName)}");

                var cardInfoContainerHTMLElements = page.Locator("div.container-infos");
                var itemName = (await cardInfoContainerHTMLElements.Locator("div.container-title > div.name > div.item-name").TextContentAsync())?.Trim();
                var englishItemName = (await cardInfoContainerHTMLElements.Locator("div.container-title > div.name > div.item-name-en").TextContentAsync())?.Trim();

                _logger.LogInformation("Starting scraping for card: {cardEnglishName}", englishItemName);

                var showMarketplaceListings = cardInfoContainerHTMLElements.Locator("div.container-show-price-mkp > div.container-price-mkp-item > div.title > span.cursor-pointer");

                //click and wait for the modal show
                await showMarketplaceListings.ClickAsync();
                await page.WaitForSelectorAsync("#full-average-price");

                var marketplaceEditionContainerModalHTMLElement = page.Locator("div#full-average-price > div.modal-content > div.body > div#container-fullprice > div.container-edition");

                var tradingCardListing = new List<TradingCardListing>();

                for (var listingIndex = 0; listingIndex < await marketplaceEditionContainerModalHTMLElement.CountAsync(); listingIndex++)
                {
                    var listingHTMLItem = marketplaceEditionContainerModalHTMLElement.Nth(listingIndex);

                    var editionPriceExtrasHTMLElement = listingHTMLItem.Locator("div.edition-price-extras");

                    var collectionName = await listingHTMLItem.Locator("div.label-edition > a").TextContentAsync() ?? string.Empty;

                    for (var extrasIndex = 0; extrasIndex < await editionPriceExtrasHTMLElement.CountAsync(); extrasIndex++)
                    {
                        var priceListingForCurrentExtra = editionPriceExtrasHTMLElement.Nth(extrasIndex);

                        var extraName = await priceListingForCurrentExtra.Locator("div.container-extras").TextContentAsync() ?? string.Empty;
                        var extraLowerPrice = (await priceListingForCurrentExtra.Locator("div.price.price-min").TextContentAsync())?.Trim() ?? string.Empty;
                        var extraMidPrice = (await priceListingForCurrentExtra.Locator("div.price.price-medium").TextContentAsync())?.Trim() ?? string.Empty;
                        var extraHigherPrice = (await priceListingForCurrentExtra.Locator("div.price.price-max").TextContentAsync())?.Trim() ?? string.Empty;

                        var listing = new TradingCardListing
                        {
                            CollectionName = collectionName,
                            CardHigherPrice = extraHigherPrice,
                            CardLowerPrice = extraLowerPrice,
                            CardMidPrice = extraMidPrice,
                            Extra = extraName
                        };

                        tradingCardListing.Add(listing);
                    }
                }
                _logger.LogInformation("Scraping done for card: {cardName}", englishItemName);

                return new MarketplaceListing
                {
                    CardName = cardName,
                    CardNameInEnglish = englishItemName,
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