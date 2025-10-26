namespace CardSarcophagus.Infra.Scraping.Models
{
    public record MarketplaceListing
    {
        public string Marketplace { get; init; }
        public string CardName { get; init; } = string.Empty;
        public string CardNameInEnglish { get; init; } = string.Empty;
        public List<TradingCardListing> TradingCardListing { get; init; } = [];
    }
    public record TradingCardListing
    {
        public string Extra { get; init; } = string.Empty;
        public string CollectionName { get; init; } = string.Empty;
        public string CardLowerPrice { get; init; } = string.Empty;
        public string CardMidPrice { get; init; } = string.Empty;
        public string CardHigherPrice { get; init; } = string.Empty;
    }
}