using CardSarcophagus.Infra.Scraping.Models;

namespace CardSarcophagus.Infra.Scraping.Interfaces
{
    public interface IPriceProviderService
    {
        Task<MarketplaceListing> GetPriceAsync(string cardName);
    }
}