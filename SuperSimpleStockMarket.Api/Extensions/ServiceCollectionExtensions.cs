using SuperSimpleStockMarket.Services;
using SuperSimpleStockMarket.Services.Interfaces;
using SuperSimpleStockMarket.Api.Repository;

namespace SuperSimpleStockMarket.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services) =>
        services
            .AddScoped<IStockService, StockService>()
            .AddScoped<IGlobalBeverageCorporationExchangeService, GlobalBeverageCorporationExchangeService>()
            .AddSingleton<IGBCEFactory, GBCEFactory>();
}
