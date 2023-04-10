namespace SuperSimpleStockMarket.Models;

/// <summary>
/// Global Beverage Corporation Exchange (GBCE)
/// </summary>
public class GlobalBeverageCorporationExchange
{
    /// <summary>
    /// GBCE Stocks collection
    /// </summary>
    /// <typeparam name="String">Stock Symbol</typeparam>
    /// <typeparam name="Stock">Stock</typeparam>
    /// <returns>Dictionary of Exchange Stocks, where key is the Stock Symbol</returns>
    public IDictionary<String, Stock> Stocks { get; set; } = new Dictionary<String, Stock>();

    /// <summary>
    /// GBCE All Share Index
    /// </summary>
    /// <value>Decimal GBCE All Share Index</value>
    public Decimal AllShareIndex { get; set; }
}
