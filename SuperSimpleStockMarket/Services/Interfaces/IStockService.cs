using SuperSimpleStockMarket.Models;

namespace SuperSimpleStockMarket.Services.Interfaces;

public interface IStockService
{
    Decimal CalculateDividendYield(ref Stock stock, Decimal price);
    Decimal CalculatePERatio(ref Stock stock, Decimal price);
    Decimal CalculateVolumeWeightedStockPrice(ref Stock stock, UInt16 tradeIntervalMinutes = 5);
    Boolean TryAddTrade(ref Stock stock, Trade trade);
}
