namespace SuperSimpleStockMarket.Models;

public class Stock
{
    public string? Symbol { get; set; }
    public StockType Type { get; set; } = StockType.Common;
    public Decimal LastDividend { get; set; }
    public Decimal? FixedDividend { get; set; } = null;
    public Decimal ParValue { get; set; }
    public Decimal? DividendYield { get; set; } = null;
    public Decimal? PERatio { get; set; } = null;
    public Decimal? VolumeWeightedPrice { get; set; } = null;
}
