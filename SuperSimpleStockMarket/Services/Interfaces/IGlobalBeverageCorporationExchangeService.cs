using SuperSimpleStockMarket.Models;

namespace SuperSimpleStockMarket.Services.Interfaces;

public interface IGlobalBeverageCorporationExchangeServiceService
{
    Boolean TryAddStock(ref GlobalBeverageCorporationExchange exchange, Stock stock);
    Decimal CalculateAllShareIndex(GlobalBeverageCorporationExchange exchange);
}
