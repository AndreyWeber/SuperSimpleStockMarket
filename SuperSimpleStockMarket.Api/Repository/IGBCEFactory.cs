using SuperSimpleStockMarket.Models;

namespace SuperSimpleStockMarket.Api.Repository;

public interface IGBCEFactory
{
    GlobalBeverageCorporationExchange GetExchange();
}
