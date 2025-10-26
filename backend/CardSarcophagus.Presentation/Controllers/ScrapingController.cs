using CardSarcophagus.Infra.Scraping.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CardSarcophagus.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
#if(!DEBUG)
[ApiExplorerSettings(IgnoreApi = true)]
#endif
    public class ScrapingController : ControllerBase
    {
        private readonly IPriceProviderService _priceProviderService;

        public ScrapingController(IPriceProviderService priceProviderService)
        {
            _priceProviderService = priceProviderService;
        }

        [HttpGet("GetCardPrice/{cardName}")]
        public async Task<IActionResult> GetCardPrice(string cardName)
        {
            var priceResult = await _priceProviderService.GetPriceAsync(cardName);
            return Ok(priceResult);
        }
    }
}