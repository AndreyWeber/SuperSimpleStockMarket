namespace SuperSimpleStockMarket.Models;

/// <summary>
/// GBCE Stock
/// </summary>
public class Stock
{
    /// <summary>
    /// Stock Exchange Symbol
    /// </summary>
    /// <value>String represents Stock name on Exchange. Examples: VOD.L, IBM.N, etc.</value>
    public String? Symbol { get; set; }
    /// <summary>
    /// Stock type
    /// </summary>
    /// <value>[StockType.Common | StockType.Preferred] Default value 'Common'</value>
    public StockType Type { get; set; } = StockType.Common;
    /// <summary>
    /// Stock Last Dividend value
    /// </summary>
    public Decimal LastDividend { get; set; }
    /// <summary>
    /// Stock Fixied Dividend value
    /// </summary>
    /// <value>Missing value represnted by null. Value represents Fixed Dividend as a percentage</value>
    public Decimal? FixedDividend { get; set; } = null;
    /// <summary>
    /// Stock Par value
    /// </summary>
    public Decimal ParValue { get; set; }
    /// <summary>
    /// Stock Didvidend Yield value
    /// </summary>
    public Decimal DividendYield { get; set; }
    /// <summary>
    /// Stock PE Ration value
    /// </summary>
    public Decimal PERatio { get; set; }
    /// <summary>
    /// Stock Volume Weighted Price value
    /// </summary>
    public Decimal VolumeWeightedPrice { get; set; }
    /// <summary>
    /// Stock trades collection
    /// </summary>
    /// <typeparam name="DateTime">Trade time stamp</typeparam>
    /// <typeparam name="Trade">Stock Trade</typeparam>
    /// <returns>Dictionary of Stock Trades, where key is the trade event time stamp</returns>
    public IDictionary<DateTime, Trade> Trades { get; set; } = new Dictionary<DateTime, Trade>();
}
