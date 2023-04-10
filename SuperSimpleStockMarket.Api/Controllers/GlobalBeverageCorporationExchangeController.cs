using SuperSimpleStockMarket.Services.Interfaces;
using SuperSimpleStockMarket.Models;
using SuperSimpleStockMarket.Api.Repository;
using Microsoft.AspNetCore.Mvc;

namespace SuperSimpleStockMarket.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GlobalBeverageCorporationExchangeController : ControllerBase
{
    private readonly ILogger<GlobalBeverageCorporationExchangeController> _logger;
    private readonly IStockService _stockService;
    private readonly IGlobalBeverageCorporationExchangeService _gbceService;
    private readonly IGBCEFactory _gbceFactory;

    public GlobalBeverageCorporationExchangeController(
        ILogger<GlobalBeverageCorporationExchangeController> logger,
        IStockService stockService,
        IGlobalBeverageCorporationExchangeService gbceService,
        IGBCEFactory gbceFactory
    )
    {
        _logger = logger;
        _stockService = stockService;
        _gbceService = gbceService;
        _gbceFactory = gbceFactory;
    }

    [HttpPost("Stocks/Add")]
    public async Task<IActionResult> AddStock([FromBody] Stock stock)
    {
        if (stock == null || String.IsNullOrWhiteSpace(stock.Symbol))
        {
            return BadRequest("Stock is null or Stock Symbol is null or empty");
        }

        try
        {
            var gbce = _gbceFactory.GetExchange();
            var result = await _gbceService.TryAddStockAsync(gbce, stock);

            return Ok(result);
        }
        catch (Exception ex)
        {
            var errMsg = $"Failed to add Stock '{stock.Symbol}'";
            _logger.LogError(ex, errMsg);
            return StatusCode(500, $"{errMsg}. Error message: {ex.Message}");
        }
    }

    [HttpGet("AllShareIndex")]
    public IActionResult GetAllShareIndex()
    {
        try
        {
            var gbce = _gbceFactory.GetExchange();
            var result = _gbceService.CalculateAllShareIndex(gbce);

            return Ok(result);
        }
        catch (Exception ex)
        {
            var errMsg = $"Failed to get GBCE All Share Index";
            _logger.LogError(ex, errMsg);
            return StatusCode(500, $"{errMsg}. Error message: {ex.Message}");
        }
    }

    [HttpPost("Stocks/{symbol}/Trades/Add")]
    public async Task<IActionResult> AddTrade(String symbol, [FromBody] Trade trade)
    {
        if (String.IsNullOrWhiteSpace(symbol))
        {
            return BadRequest("Stock symbol is null or empty");
        }

        if (trade == null || String.IsNullOrWhiteSpace(trade.Symbol))
        {
            return BadRequest(
                "Trade is null or Trade Stock Symbol is null or empty");
        }

        Stock stock;
        try
        {
            var gbce = _gbceFactory.GetExchange();
            if (!gbce.Stocks.TryGetValue(symbol, out stock!))
            {
                var errMsg = $"Stock '{symbol}' not found on GBCE";
                _logger.LogError(errMsg);
                return NotFound(errMsg);
            }

            var result = await _stockService.TryAddTradeAsync(stock, trade);

            return Ok(result);
        }
        catch (Exception ex)
        {
            var errMsg = $"Failed to add Trade to Stock '{symbol}'";
            _logger.LogError(ex, errMsg);
            return StatusCode(500, $"{errMsg}. Error message: {ex.Message}");
        }
    }

    [HttpGet("Stocks/{symbol}/Price/{price:decimal}/DividendYield")]
    public IActionResult GetStockDividendYield(String symbol, Decimal price)
    {
        if (String.IsNullOrWhiteSpace(symbol))
        {
            return BadRequest("Stock symbol is null or empty");
        }

        Stock stock;
        try
        {
            var gbce = _gbceFactory.GetExchange();
            if (!gbce.Stocks.TryGetValue(symbol, out stock!))
            {
                var errMsg = $"Stock '{symbol}' not found on GBCE";
                _logger.LogError(errMsg);
                return NotFound(errMsg);
            }

            var result = _stockService.CalculateDividendYield(ref stock, price);

            return Ok(result);
        }
        catch (Exception ex)
        {
            var errMsg = $"Failed to get DividendYield for Stock '{symbol}'";
            _logger.LogError(ex, errMsg);
            return StatusCode(500, $"{errMsg}. Error message: {ex.Message}");
        }
    }

    [HttpGet("Stocks/{symbol}/Price/{price:decimal}/PERatio")]
    public IActionResult GetStockPERatio(String symbol,Decimal price)
    {
        if (String.IsNullOrWhiteSpace(symbol))
        {
            return BadRequest("Stock symbol is null or empty");
        }

        Stock stock;
        try
        {
            var gbce = _gbceFactory.GetExchange();
            if (!gbce.Stocks.TryGetValue(symbol, out stock!))
            {
                var errMsg = $"Stock '{symbol}' not found on GBCE";
                _logger.LogError(errMsg);
                return NotFound(errMsg);
            }

            var result = _stockService.CalculatePERatio(ref stock, price);

            return Ok(result);
        }
        catch (Exception ex)
        {
            var errMsg = $"Failed to get PERatio for Stock '{symbol}'";
            _logger.LogError(ex, errMsg);
            return StatusCode(500, $"{errMsg}. Error message: {ex.Message}");
        }
    }

    [HttpGet("Stocks/{symbol}/VolumeWeightedStockPrice")]
    [HttpGet("Stocks/{symbol}/TradeIntervalMinutes/{tradeIntervalMinutes:int}/VolumeWeightedStockPrice")]
    public IActionResult GetVolumeWeightedStockPrice(String symbol, Int32 tradeIntervalMinutes = 5)
    {
        if (String.IsNullOrWhiteSpace(symbol))
        {
            return BadRequest("Stock symbol is null or empty");
        }

        if (tradeIntervalMinutes <= 0)
        {
            return BadRequest("TradeIntervalMinutes have to be greater than zero");
        }

        Stock stock;
        try
        {
            var gbce = _gbceFactory.GetExchange();
            if (!gbce.Stocks.TryGetValue(symbol, out stock!))
            {
                var errMsg = $"Stock '{symbol}' not found on GBCE";
                _logger.LogError(errMsg);
                return NotFound(errMsg);
            }

            var result = _stockService
                .CalculateVolumeWeightedStockPrice(ref stock, (UInt16)tradeIntervalMinutes);

            return Ok(result);
        }
        catch (Exception ex)
        {
            var errMsg = $"Failed to get VolumeWeightedStockPrice for Stock '{symbol}'";
            _logger.LogError(ex, errMsg);
            return StatusCode(500, $"{errMsg}. Error message: {ex.Message}");
        }
    }
}
