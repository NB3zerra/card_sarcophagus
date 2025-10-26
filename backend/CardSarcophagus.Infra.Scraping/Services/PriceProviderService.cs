using System.Text.Json;
using CardSarcophagus.Infra.Scraping.Interfaces;
using CardSarcophagus.Infra.Scraping.Models;

namespace CardSarcophagus.Infra.Scraping.Services
{

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
                    var fileName = $"scraping_results_{DateTime.Now:yyyyMMdd_HHmmss}.json";
                    var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ScrapingResults", fileName);

                    // Ensure directory exists
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                    // Save to JSON file
                    var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
                    await File.WriteAllTextAsync(filePath, JsonSerializer.Serialize(result, jsonOptions));

                    return result;
                }
            }

            return null;
        }
    }
}