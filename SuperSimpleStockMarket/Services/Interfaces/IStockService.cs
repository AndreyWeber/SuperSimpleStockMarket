using SuperSimpleStockMarket.Models;

namespace SuperSimpleStockMarket.Services.Interfaces;

public interface IStockService
{
    Decimal GetDividendYield(Stock stock, Decimal price);
    Decimal CalculatePERatio(Stock stock, Decimal price);
    Decimal CalculateVolumeWeightedStockPrice(Stock stock, IEnumerable<Trade> stockTrades);
}
