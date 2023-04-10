using SuperSimpleStockMarket.Models;

namespace SuperSimpleStockMarket.Services.Interfaces;

/// <summary>
/// Global Beverage Corporation Exchange (GBCE) Service represents
/// set of operations that can be carried out on Stocks traded
/// on GBCE
/// </summary>
public interface IGlobalBeverageCorporationExchangeService
{
    /// <summary>
    /// Try to add Stock into GBCE Stocks collection.
    /// Method is thread safe
    /// </summary>
    /// <param name="exchange">GBCE</param>
    /// <param name="stock">Stock to add</param>
    /// <returns>
    /// 'true' if Stock was successfully added, 'false' otherwise. Stock won't
    /// be added if Stock Symbol is null or empty string or if Stock with the
    /// same Symbol already exists
    /// </returns>
    Task<Boolean> TryAddStockAsync(GlobalBeverageCorporationExchange exchange, Stock stock);
    /// <summary>
    /// Calculate GBCE All Share Index using the geometric mean of the Volume
    /// Weighted Stock Price for all stocks
    /// </summary>
    /// <param name="exchange">GBCE</param>
    /// <returns>GBCE All Share Index value</returns>
    Decimal CalculateAllShareIndex(GlobalBeverageCorporationExchange exchange);
}
