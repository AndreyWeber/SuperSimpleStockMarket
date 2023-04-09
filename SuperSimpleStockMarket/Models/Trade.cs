namespace SuperSimpleStockMarket.Models;

public class Trade
{
    public String? Symbol { get; set; }
    public DateTime TimeStamp { get; set; }
    public Int32 Quantity { get; set; }
    public Decimal Price { get; set; }
    public TradeType Type { get; set; }
}
