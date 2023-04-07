using SuperSimpleStockMarket.Models;
using SuperSimpleStockMarket.Services.Interfaces;

namespace SuperSimpleStockMarket.Services;

public class GlobalBeverageCorporationExchange : IGlobalBeverageCorporationExchange
{
    public decimal CalculateAllShareIndex(GlobalBeverageCorporationExchange exchange)
    {
        throw new NotImplementedException();
    }

    public bool TryAddStock(GlobalBeverageCorporationExchange exchange, Stock stock)
    {
        throw new NotImplementedException();
    }

    public bool TryAddTrade(GlobalBeverageCorporationExchange exchange, Trade trade)
    {
        throw new NotImplementedException();
    }
}
