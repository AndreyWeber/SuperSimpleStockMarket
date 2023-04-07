using SuperSimpleStockMarket.Models;

namespace SuperSimpleStockMarket.Services.Interfaces;

public interface IGlobalBeverageCorporationExchange
{
    Boolean TryAddStock(GlobalBeverageCorporationExchange exchange, Stock stock);
    Boolean TryAddTrade(GlobalBeverageCorporationExchange exchange, Trade trade);
    Decimal CalculateAllShareIndex(GlobalBeverageCorporationExchange exchange);
}
