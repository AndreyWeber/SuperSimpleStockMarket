using SuperSimpleStockMarket.Models;
using SuperSimpleStockMarket.Services.Interfaces;
using SuperSimpleStockMarket.Common;

namespace SuperSimpleStockMarket.Services;

public class StockService : IStockService
{
    private readonly ILogger<StockService> _logger;

    public StockService(ILogger<StockService> logger)
    {
        _logger = logger;
    }

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
    ///
    /// </summary>
    /// <param name="stock"></param>
    /// <param name="tradeInterval">Trade interval. Default value is 5 minutes</param>
    /// <returns></returns>
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

    public Boolean TryAddTrade(ref Stock stock, Trade trade)
    {
        Throw.IfNull(nameof(stock), stock);
        Throw.IfNull(nameof(trade), trade);

        stock.Trades ??= new Dictionary<DateTime, Trade>();

        if (String.IsNullOrWhiteSpace(trade.Symbol))
        {
            _logger.LogWarning("Trade wasn't added to Stock '{Symbol}'. Trade.Symbol is null or empty",
                stock.Symbol);
            return false;
        }

        return stock.Trades.TryAdd(trade.TimeStamp, trade);
    }

    private static Decimal GetCommonDividendYield(Decimal lastDividend, Decimal price) =>
        price.Equals(Decimal.Zero)
            ? Decimal.Zero
            : Decimal.Divide(lastDividend, price);

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

    private static Decimal GetVolumeWeightedStockPrice(IList<Trade> trades) =>
        trades.Any()
            ? Decimal.Divide(
                trades.Sum(t => Decimal.Multiply(t.Price, t.Quantity)),
                trades.Sum(t => t.Quantity)
            )
            : Decimal.Zero;
}
