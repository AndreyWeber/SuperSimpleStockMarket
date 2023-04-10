namespace SuperSimpleStockMarket.Models;

/// <summary>
/// GBCE Stock Trade
/// </summary>
public class Trade
{
    /// <summary>
    /// Stock Symbol
    /// </summary>
    /// <value>Represents Stock Exchange Symbol to determine relation between Stock and Trade</value>
    public String? Symbol { get; set; }
    /// <summary>
    /// Trade time stamp
    /// </summary>
    /// <value>Represents time stamp when Stock trade event occured</value>
    public DateTime TimeStamp { get; set; }
    /// <summary>
    /// Quantity of Stocks
    /// </summary>
    /// <value>Represents number of Stocks sold/bought duiring the trade event</value>
    public Int32 Quantity { get; set; }
    /// <summary>
    /// Trade price value
    /// </summary>
    /// <value>Price paid for <paramref name="Quantity"/> of Stocks</value>
    public Decimal Price { get; set; }
    /// <summary>
    /// Trade type
    /// </summary>
    /// <value>Represents Trade buy or sell indicator. Allowed values [TradeType.Buy | TradeType.Sell]</value>
    public TradeType Type { get; set; }
}
