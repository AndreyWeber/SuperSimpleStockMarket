using SuperSimpleStockMarket.Models;
using SuperSimpleStockMarket.Services.Interfaces;
using SuperSimpleStockMarket.Common;

namespace SuperSimpleStockMarket.Services;

public class StockService : IStockService
{
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
    public Decimal CalculateVolumeWeightedStockPrice(ref Stock stock, Int32 tradeIntervalMinutes = 5)
    {
        Throw.IfNull(nameof(stock), stock);

        if (stock.Trades == null)
        {
            // TODO: Add logging
            throw new InvalidOperationException(
                "Stock Trades collection was not initialized. It is equall to null");
        }

        var tradeIntervalStart = DateTime.Now;
        var tradeIntervalEnd = tradeIntervalStart.AddMinutes(tradeIntervalMinutes);
        var tradesInTheInterval = stock.Trades
            .Where(kvp => kvp.Key >= tradeIntervalStart && kvp.Key <= tradeIntervalEnd)
            .Select(kvp => kvp.Value)
            .ToList();
        try
        {
            var result = GetVolumeWeightedStockPrice(tradesInTheInterval);
            stock.VolumeWeightedPrice = result;

            return result;
        }
        catch (DivideByZeroException ex)
        {
            // TODO: Add logging
            throw;
        }
        catch (Exception ex)
        {
            // TODO: Add logging
            throw;
        }
    }

    public Boolean TryAddTrade(ref Stock stock, Trade trade)
    {
        Throw.IfNull(nameof(stock), stock);
        Throw.IfNull(nameof(trade), trade);

        stock.Trades ??= new Dictionary<DateTime, Trade>();

        if (String.IsNullOrWhiteSpace(trade.Symbol))
        {
            // TODO: Add logging
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
