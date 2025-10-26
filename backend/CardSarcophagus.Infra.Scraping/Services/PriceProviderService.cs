namespace CardSarcophagus.Infra.Scraping.Services
{
    using CardSarcophagus.Infra.Scraping.Interfaces;
    using CardSarcophagus.Infra.Scraping.Models;

    public class PriceProviderService : IPriceProviderService
    {
        private readonly IEnumerable<IPriceProvider> _priceProviders;

        public PriceProviderService(IEnumerable<IPriceProvider> priceProviders)
        {
            _priceProviders = priceProviders;
        }

        public async Task<MarketplaceListing> GetPriceAsync(string cardName)
        {
            foreach (var provider in _priceProviders)
            {
                var result = await provider.GetPriceAsync(cardName);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }
    }
}