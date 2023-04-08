using SuperSimpleStockMarket.Models;
using SuperSimpleStockMarket.Services.Interfaces;
using SuperSimpleStockMarket.Common;
using DecimalMath;

namespace SuperSimpleStockMarket.Services;

public class GlobalBeverageCorporationExchangeService : IGlobalBeverageCorporationExchangeServiceService
{
    public Decimal CalculateAllShareIndex(GlobalBeverageCorporationExchange exchange)
    {
        Throw.IfNull(nameof(exchange), exchange);

        if (exchange.Stocks == null)
        {
            // TODO: Add logging
            throw new InvalidOperationException(
                "Exchange Stocks colleaction was not initialized. It is eqaul to null");
        }

        try
        {
            var stocks = exchange.Stocks
                .Where(kvp => kvp.Value != null)
                .Select(kvp => kvp.Value)
                .ToList();

            var sumOfLogarithms = GetSumOfVolumeWeightedPriceLogarithms(stocks);
            var averageOfLogarithms = Decimal.Divide(sumOfLogarithms, stocks.Count);
            var allShareIndex = DecimalEx.Exp(averageOfLogarithms);

            exchange.AllShareIndex = allShareIndex;

            return allShareIndex;
        }
        catch (Exception ex)
        {
            // TODO: Add logging
            throw;
        }
    }

    public Boolean TryAddStock(ref GlobalBeverageCorporationExchange exchange, Stock stock)
    {
        Throw.IfNull(nameof(exchange), exchange);
        Throw.IfNull(nameof(stock), stock);

        if (String.IsNullOrWhiteSpace(stock.Symbol))
        {
            // TODO: Add logging
            return false;
        }

        exchange.Stocks ??= new Dictionary<String, Stock>();

        return exchange.Stocks.TryAdd(stock.Symbol, stock);
    }

    private static Decimal GetSumOfVolumeWeightedPriceLogarithms(IEnumerable<Stock> stocks) =>
        stocks.Aggregate(
            Decimal.Zero,
            (accum, stock) => Decimal.Compare(stock.VolumeWeightedPrice, Decimal.Zero) <= 0
                ? throw new InvalidOperationException(
                    $"Stock '{stock.Symbol}' has negative or zero VolumeWeightedPrice value: '{stock.VolumeWeightedPrice}'")
                : Decimal.Add(accum, DecimalEx.Log(stock.VolumeWeightedPrice))
        );
}
