namespace SuperSimpleStockMarket.Models;

public class GlobalBeverageCorporationExchange
{
    public IDictionary<String, Stock> TradedStocks { get; set; } = new Dictionary<String, Stock>();
    public IDictionary<String, IEnumerable<Trade>> Trades { get; set; } = new Dictionary<String, IEnumerable<Trade>>();
    public Decimal? AllShareIndex { get; set; } = null;
}
