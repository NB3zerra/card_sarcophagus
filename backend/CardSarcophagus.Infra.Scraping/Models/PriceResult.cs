namespace CardSarcophagus.Infra.Scraping.Models
{
    public record PriceResult
    {
        public decimal Price { get; init; }
        public string Currency { get; init; }
        public string Source { get; init; }
        public DateTime FetchedAt { get; init; }
    }
}