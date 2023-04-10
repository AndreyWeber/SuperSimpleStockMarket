using SuperSimpleStockMarket.Models;
using SuperSimpleStockMarket.Services.Interfaces;
using SuperSimpleStockMarket.Common;

namespace SuperSimpleStockMarket.Services;

/// <summary>
/// Stock Service represents set of operations that can
/// be carried out on Stock that can be traded on GBCE
/// </summary>
public class StockService : IStockService
{
    private static readonly SemaphoreSlim? Semaphore = new (1, 1);

    private readonly ILogger<StockService> _logger;

    public StockService(ILogger<StockService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Calculate Stock Dividend Yield value
    /// </summary>
    /// <param name="stock">Stock</param>
    /// <param name="price">Price value</param>
    /// <returns>Calculated Stock Dividend Yield value</returns>
    public Decimal CalculateDividendYield(ref Stock stock, Decimal price)
    {
        Throw.IfNull(nameof(stock), stock);

        var result = stock.Type switch
        {
            StockType.Common => GetCommonDividendYield(stock.LastDividend, price),
            StockType.Preferred => GetPreferredDividendYield(stock.FixedDividend, stock.ParValue, price),
            _ => throw new ArgumentOutOfRangeException($"Unexpected StockType: {stock.Type}")
        };

        stock.DividendYield = result;

        return result;
    }

    /// <summary>
    /// Calculate Stock P/E Ratio value
    /// </summary>
    /// <param name="stock">Stock</param>
    /// <param name="price">Price value</param>
    /// <returns>Stock P/E Ratio value</returns>
    public Decimal CalculatePERatio(ref Stock stock, Decimal price)
    {
        Throw.IfNull(nameof(stock), stock);

        var result = stock.DividendYield.Equals(Decimal.Zero)
            ? Decimal.Zero
            : Decimal.Divide(price, stock.DividendYield);

        stock.PERatio = result;

        return result;
    }

    /// <summary>
    /// Calculate Volume Weighted Stock Price based on trades in past N minutes
    /// </summary>
    /// <param name="stock">Stock</param>
    /// <param name="tradeIntervalMinutes">
    /// Past trade interval in minutes. Default interval is 5 minutes
    /// </param>
    /// <returns>Volume Weighted Stock Price value</returns>
    public Decimal CalculateVolumeWeightedStockPrice(ref Stock stock, UInt16 tradeIntervalMinutes = 5)
    {
        Throw.IfNull(nameof(stock), stock);

        if (stock.Trades == null)
        {
            var errMsg = $"Stock '{stock.Symbol}' Trades collection is not initialized";
            _logger.LogError(errMsg);
            throw new InvalidOperationException(errMsg);
        }

        var tradeIntervalEnd = DateTime.Now;
        var tradeIntervalStart = tradeIntervalEnd.AddMinutes(-tradeIntervalMinutes);
        var tradesInTheInterval = stock.Trades
            .Where(kvp => kvp.Key >= tradeIntervalStart && kvp.Key <= tradeIntervalEnd)
            .Select(kvp => kvp.Value)
            .ToList();
        try
        {
            var volumeWeightedPrice = GetVolumeWeightedStockPrice(tradesInTheInterval);

            stock.VolumeWeightedPrice = volumeWeightedPrice;

            return volumeWeightedPrice;
        }
        catch (DivideByZeroException ex)
        {
            var errMsg = $"Stock '{stock.Symbol}': one of the trades has Quantity equal to zero";
            _logger.LogError(ex, errMsg);
            throw new DivideByZeroException(errMsg, ex);
        }
        catch (Exception ex)
        {
            var errMsg = "Unexpected error occurred";
            _logger.LogError(ex, errMsg);
            throw new Exception(errMsg, ex);
        }
    }

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
    public async Task<Boolean> TryAddTradeAsync(Stock stock, Trade trade)
    {
        Throw.IfNull(nameof(stock), stock);
        Throw.IfNull(nameof(trade), trade);
        try
        {
            await Semaphore!.WaitAsync();

            stock.Trades ??= new Dictionary<DateTime, Trade>();

            if (String.IsNullOrWhiteSpace(trade.Symbol))
            {
                _logger.LogWarning("Trade wasn't added to Stock '{Symbol}'. Trade.Symbol is null or empty",
                    stock.Symbol);
                return false;
            }

            if (!stock.Symbol!.Equals(trade.Symbol))
            {
                _logger.LogWarning("Trade wasn't added to Stock '{StockSymbol}'. Trade.Symbol '{TradeSymbol}' is not equal to the Stock.Symbol",
                    stock.Symbol, trade.Symbol);
                return false;
            }

            return stock.Trades.TryAdd(trade.TimeStamp, trade);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add new Trade to Stock '{Symbol}'", stock.Symbol);
            return false;
        }
        finally
        {
            Semaphore?.Release();
        }
    }

    /// <summary>
    /// Get Stock Dividend Yield value for the Common stock type
    /// </summary>
    /// <param name="lastDividend">Stock Last Dividend value</param>
    /// <param name="price">Price value</param>
    /// <returns>Stock Dividend Yield value for the Common stock type</returns>
    private static Decimal GetCommonDividendYield(Decimal lastDividend, Decimal price) =>
        price.Equals(Decimal.Zero)
            ? Decimal.Zero
            : Decimal.Divide(lastDividend, price);

    /// <summary>
    /// Get Stock Dividend Yield value for the Preferred stock type
    /// </summary>
    /// <param name="fixedDividend">Stock Fixed Dividend value</param>
    /// <param name="parValue">Stock Par Value</param>
    /// <param name="price">Price value</param>
    /// <returns></returns>
    private static Decimal GetPreferredDividendYield(Decimal? fixedDividend, Decimal parValue, Decimal price)
    {
        if (!fixedDividend.HasValue)
        {
            return Decimal.Zero;
        }

        var result = price.Equals(Decimal.Zero)
            ? Decimal.Zero
            : Decimal.Divide(
                Decimal.Multiply(fixedDividend.Value, parValue),
                price
            );

        return result;
    }

    /// <summary>
    /// Get Volume Weighted Stock Price value
    /// </summary>
    /// <param name="trades">Stock Trades collection</param>
    /// <returns>Volume Weighted Stock Price value</returns>
    private static Decimal GetVolumeWeightedStockPrice(IList<Trade> trades) =>
        trades.Any()
            ? Decimal.Divide(
                trades.Sum(t => Decimal.Multiply(t.Price, t.Quantity)),
                trades.Sum(t => t.Quantity)
            )
            : Decimal.Zero;
}
