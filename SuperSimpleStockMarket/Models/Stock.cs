namespace SuperSimpleStockMarket.Models;

public class Stock
{
    public String? Symbol { get; set; }
    public StockType Type { get; set; } = StockType.Common;
    public Decimal LastDividend { get; set; }
    public Decimal? FixedDividend { get; set; } = null;
    public Decimal ParValue { get; set; }
    public Decimal DividendYield { get; set; }
    public Decimal PERatio { get; set; }
    public Decimal VolumeWeightedPrice { get; set; }
    public IDictionary<DateTime, Trade> Trades { get; set; } = new Dictionary<DateTime, Trade>();
}
