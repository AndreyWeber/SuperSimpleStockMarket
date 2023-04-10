using SuperSimpleStockMarket.Models;

namespace SuperSimpleStockMarket.Services.Interfaces;

/// <summary>
/// Stock Service represents set of operations that can
/// be carried out on Stock that can be traded on GBCE
/// </summary>
public interface IStockService
{
    /// <summary>
    /// Calculate Stock Dividend Yield value
    /// </summary>
    /// <param name="stock">Stock</param>
    /// <param name="price">Price value</param>
    /// <returns>Calculated Stock Dividend Yield value</returns>
    Decimal CalculateDividendYield(ref Stock stock, Decimal price);
    /// <summary>
    /// Calculate Stock P/E Ratio value
    /// </summary>
    /// <param name="stock">Stock</param>
    /// <param name="price">Price value</param>
    /// <returns>Stock P/E Ratio value</returns>
    Decimal CalculatePERatio(ref Stock stock, Decimal price);
    /// <summary>
    /// Calculate Volume Weighted Stock Price based on trades in past N minutes
    /// </summary>
    /// <param name="stock">Stock</param>
    /// <param name="tradeIntervalMinutes">
    /// Past trade interval in minutes. Default interval is 5 minutes
    /// </param>
    /// <returns>Volume Weighted Stock Price value</returns>
    Decimal CalculateVolumeWeightedStockPrice(ref Stock stock, UInt16 tradeIntervalMinutes = 5);
    /// <summary>
    /// Try to add Trade into Stock trades collection.
    /// Method is thread safe
    /// </summary>
    /// <param name="stock">Stock</param>
    /// <param name="trade">Trade</param>
    /// <returns>
    /// 'true' if Trade was successfully added, 'false' otherwise. Trade won't
    /// be added if Trade Stock Symbol is null or empty or if Trade with the
    /// same time stamp already exists
    /// </returns>
    Task<Boolean> TryAddTradeAsync(Stock stock, Trade trade);
}
