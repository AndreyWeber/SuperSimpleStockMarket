namespace SuperSimpleStockMarket.Models;

public class GlobalBeverageCorporationExchange
{
    public IDictionary<String, Stock> Stocks { get; set; } = new Dictionary<String, Stock>();
    public Decimal? AllShareIndex { get; set; } = null;
}
