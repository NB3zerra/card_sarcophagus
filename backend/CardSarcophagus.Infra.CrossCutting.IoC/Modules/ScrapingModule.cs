using CardSarcophagus.Infra.Scraping.Adapters.LigaYgo;
using CardSarcophagus.Infra.Scraping.Interfaces;
using CardSarcophagus.Infra.Scraping.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;

namespace CardSarcophagus.Infra.CrossCutting.IoC.Modules
{
    public static class ScrapingModule
    {
        public static IServiceCollection RegisterScrapingServices(this IServiceCollection services)
        {
            services.RegisterPlaywright();

            services.AddHttpClient();
            services.AddTransient<IPriceProvider, LigaYgoAdapter>();


            services.AddScoped<IPriceProviderService, PriceProviderService>();

            return services;
        }

        private static IServiceCollection RegisterPlaywright(this IServiceCollection services)
        {
            services.AddSingleton(provider => Playwright.CreateAsync().GetAwaiter().GetResult());

            services.AddSingleton(provider =>
            {
                var playwright = provider.GetRequiredService<IPlaywright>();
                return playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true }).GetAwaiter().GetResult();
            });

            return services;
        }
    }
}