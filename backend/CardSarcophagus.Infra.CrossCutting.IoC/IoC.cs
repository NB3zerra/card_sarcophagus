using CardSarcophagus.Infra.CrossCutting.IoC.Modules;
using Microsoft.Extensions.DependencyInjection;

namespace CardSarcophagus.Infra.CrossCutting.IoC;

public static class IoC
{
    public static IServiceCollection RegisterScrapingServices(this IServiceCollection services)
    {
        ScrapingModule.RegisterScrapingServices(services);

        return services;
    }
}