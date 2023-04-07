namespace SuperSimpleStockMarket.Models;

public class GlobalBeverageCorporationExchange
{
    public IEnumerable<Stock> TradedStocks { get; set; } = Enumerable.Empty<Stock>();
    public IEnumerable<Trade> Trades { get; set; } = Enumerable.Empty<Trade>();
    public Decimal? AllShareIndex { get; set; } = null;
}
