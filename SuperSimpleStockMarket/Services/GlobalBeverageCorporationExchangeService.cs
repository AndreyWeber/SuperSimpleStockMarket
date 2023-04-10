using SuperSimpleStockMarket.Models;
using SuperSimpleStockMarket.Services.Interfaces;
using SuperSimpleStockMarket.Common;
using DecimalMath;

namespace SuperSimpleStockMarket.Services;

/// <summary>
/// Global Beverage Corporation Exchange (GBCE) Service represents
/// set of operations that can be carried out on Stocks traded
/// on GBCE
/// </summary>
public class GlobalBeverageCorporationExchangeService : IGlobalBeverageCorporationExchangeService
{
    private readonly ILogger<GlobalBeverageCorporationExchangeService> _logger;

    public GlobalBeverageCorporationExchangeService(
        ILogger<GlobalBeverageCorporationExchangeService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Calculate GBCE All Share Index using the geometric mean of the Volume
    /// Weighted Stock Price for all stocks
    /// </summary>
    /// <param name="exchange">GBCE</param>
    /// <returns>GBCE All Share Index value</returns>
    public Decimal CalculateAllShareIndex(GlobalBeverageCorporationExchange exchange)
    {
        Throw.IfNull(nameof(exchange), exchange);

        if (exchange.Stocks == null)
        {
            const string errMsg = "Exchange Stocks collection is not initialized";
            _logger.LogError(errMsg);
            throw new InvalidOperationException(errMsg);
        }

        try
        {
            var stocks = exchange.Stocks
                .Where(kvp => kvp.Value != null)
                .Select(kvp => kvp.Value)
                .ToList();
            if (!stocks.Any())
            {
                exchange.AllShareIndex = Decimal.Zero;
                return Decimal.Zero;
            }

            var sumOfLogarithms = GetSumOfVolumeWeightedPriceLogarithms(stocks);
            var averageOfLogarithms = Decimal.Divide(sumOfLogarithms, stocks.Count);
            var allShareIndex = DecimalEx.Exp(averageOfLogarithms);

            exchange.AllShareIndex = allShareIndex;

            return allShareIndex;
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            const string errMsg = "Unexpected error occurred";
            _logger.LogError(ex, errMsg);
            throw new Exception(errMsg, ex);
        }
    }

    /// <summary>
    /// Try to add Stock into GBCE Stocks collection
    /// </summary>
    /// <param name="exchange">GBCE</param>
    /// <param name="stock">Stock to add</param>
    /// <returns>
    /// 'true' if Stock was successfully added, 'false' otherwise. Stock won't
    /// be added if Stock Symbol is null or empty string or if Stock with the
    /// same Symbol already exists
    /// </returns>
    public Boolean TryAddStock(ref GlobalBeverageCorporationExchange exchange, Stock stock)
    {
        Throw.IfNull(nameof(exchange), exchange);
        Throw.IfNull(nameof(stock), stock);

        if (String.IsNullOrWhiteSpace(stock.Symbol))
        {
            _logger.LogWarning("Stock wasn't added to Exchange stocks collection. Stock.Symbol is null or empty");
            return false;
        }

        exchange.Stocks ??= new Dictionary<String, Stock>();

        return exchange.Stocks.TryAdd(stock.Symbol, stock);
    }

    /// <summary>
    /// Calculate sum of Volume Weighted Price natural logarithms value
    /// </summary>
    /// <param name="stocks">Collection of Stocks</param>
    /// <returns>Sum of Volume Weighted Price natural logarithms</returns>
    private static Decimal GetSumOfVolumeWeightedPriceLogarithms(IEnumerable<Stock> stocks) =>
        stocks.Aggregate(
            Decimal.Zero,
            (accum, stock) => Decimal.Compare(stock.VolumeWeightedPrice, Decimal.Zero) <= 0
                ? throw new InvalidOperationException(
                    $"Stock '{stock.Symbol}' has negative or zero VolumeWeightedPrice value: '{stock.VolumeWeightedPrice}'")
                : Decimal.Add(accum, DecimalEx.Log(stock.VolumeWeightedPrice))
        );
}
