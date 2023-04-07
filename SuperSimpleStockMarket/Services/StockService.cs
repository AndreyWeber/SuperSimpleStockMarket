using SuperSimpleStockMarket.Models;
using SuperSimpleStockMarket.Services.Interfaces;

namespace SuperSimpleStockMarket.Services;

public class StockService : IStockService
{
    public decimal GetDividendYield(Stock stock, decimal price)
    {
        throw new NotImplementedException();
    }

    public decimal CalculatePERatio(Stock stock, decimal price)
    {
        throw new NotImplementedException();
    }

    public decimal CalculateVolumeWeightedStockPrice(Stock stock, IEnumerable<Trade> stockTrades)
    {
        throw new NotImplementedException();
    }
}
