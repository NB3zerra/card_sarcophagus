using CardSarcophagus.Infra.Scraping.Models;

namespace CardSarcophagus.Infra.Scraping.Interfaces
{
    public interface IPriceProvider
    {
        Task<MarketplaceListing> GetPriceAsync(string cardName, string edition = null, string condition = null, CancellationToken cancellationToken = default);
    }
}