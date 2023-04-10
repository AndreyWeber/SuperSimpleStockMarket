using SuperSimpleStockMarket.Models;

namespace SuperSimpleStockMarket.Api.Repository;

public class GBCEFactory : IGBCEFactory
{
    private GlobalBeverageCorporationExchange? _exchange;

    public GlobalBeverageCorporationExchange GetExchange()
    {
        _exchange ??= new();
        return _exchange;
    }
}
