using SuperSimpleStockMarket.Services;
using SuperSimpleStockMarket.Services.Interfaces;
using SuperSimpleStockMarket.Models;
using Microsoft.Extensions.Logging;
using FluentAssertions;
using Moq;

namespace SuperSimpleStockMarket.Tests.Services;

public class GlobalBeverageCorporationExchangeServiceTests
{
    private readonly IGlobalBeverageCorporationExchangeService _stockService;
    private readonly ILogger<GlobalBeverageCorporationExchangeService> _logger;

    public GlobalBeverageCorporationExchangeServiceTests()
    {
        _logger = new Mock<ILogger<GlobalBeverageCorporationExchangeService>>().Object;
        _stockService = new GlobalBeverageCorporationExchangeService(_logger);
    }

    #region CalculateAllShareIndex() tests

    [Fact]
    public void CalculateAllShareIndex_NullExchange_Throws_ArgumentNullException()
    {
        // Arrange
        GlobalBeverageCorporationExchange exchange = null!;

        // Act
        var action = () => _stockService.CalculateAllShareIndex(exchange);

        // Assert
        action.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("*Argument cannot be null*");
    }

    [Fact]
    public void CalculateAllShareIndex_NullStocks_Throws_InvalidOperationException()
    {
        // Arrange
        var exchange = new GlobalBeverageCorporationExchange { Stocks = null! };

        // Act
        var action = () => _stockService.CalculateAllShareIndex(exchange);

        // Assert
        action.Should()
            .Throw<InvalidOperationException>()
            .WithMessage("*Exchange Stocks collection is not initialized*");
    }

    [Fact]
    public void CalculateAllShareIndex_EmptyStocks_ZeroAllShareIndex()
    {
        // Arrange
        var exchange = new GlobalBeverageCorporationExchange { Stocks = new Dictionary<string, Stock>() };

        // Act
        var actualAllShareIndex = _stockService.CalculateAllShareIndex(exchange);

        // Assert
        actualAllShareIndex.Should().Be(Decimal.Zero);
        exchange.AllShareIndex.Should().Be(Decimal.Zero);
    }

    [Theory]
    [MemberData(nameof(ExchangeData))]
    public void CalculateAllShareIndex_WithStocks_CalculatesCorrectAllShareIndex(
        GlobalBeverageCorporationExchange exchange,
        Decimal expectedAllShareIndex,
        Decimal expectedPrecision
    )
    {
        // Arrange

        // Act
        var allShareIndex = _stockService.CalculateAllShareIndex(exchange);

        // Assert
        allShareIndex.Should().BeApproximately(expectedAllShareIndex, expectedPrecision);
        exchange.AllShareIndex.Should().BeApproximately(expectedAllShareIndex, expectedPrecision);
    }

    [Fact]
    public void CalculateAllShareIndex_ZeroOrNegativeVolumeWeightedPrice_ThrowsInvalidOperationException()
    {
        // Arrange
        var stockA = new Stock { Symbol = "A", VolumeWeightedPrice = 100.0m };
        var stockB = new Stock { Symbol = "B", VolumeWeightedPrice = Decimal.Zero };
        var exchange = new GlobalBeverageCorporationExchange
        {
            Stocks = new Dictionary<string, Stock>
            {
                { stockA.Symbol, stockA },
                { stockB.Symbol, stockB }
            }
        };

        // Act
        var action = () => _stockService.CalculateAllShareIndex(exchange);

        // Assert
        action.Should()
            .Throw<InvalidOperationException>()
            .WithMessage("*has negative or zero VolumeWeightedPrice value*");
    }

    public static IEnumerable<object[]> ExchangeData =>
        new List<object[]>
        {
            new object[]
            {
                new GlobalBeverageCorporationExchange
                {
                    Stocks = new Dictionary<string, Stock>
                    {
                        { "A", new Stock { Symbol = "A", VolumeWeightedPrice = 100.0m } },
                        { "B", new Stock { Symbol = "B", VolumeWeightedPrice = 200.0m } },
                        { "C", new Stock { Symbol = "C", VolumeWeightedPrice = 300.0m } },
                    }
                },
                181.7120592832m, // expectedAllShareIndex
                0.71m // expectedPrecision
            }
        };

    #endregion CalculateAllShareIndex() tests

    #region TryAddStock() tests

    [Fact]
    public void TryAddStock_NullExchange_Throws_ArgumentNullException()
    {
        // Arrange
        GlobalBeverageCorporationExchange exchange = null!;
        var stock = new Stock { Symbol = "A", VolumeWeightedPrice = 100.0m };

        // Act
        var action = () => _stockService.TryAddStock(ref exchange, stock);

        // Assert
        action.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("*Argument cannot be null*");
    }

    [Fact]
    public void TryAddStock_NullStock_Throws_ArgumentNullException()
    {
        // Arrange
        var exchange = new GlobalBeverageCorporationExchange { Stocks = new Dictionary<string, Stock>() };
        Stock stock = null!;

        // Act
        var action = () => _stockService.TryAddStock(ref exchange, stock);

        // Assert
        action.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("*Argument cannot be null*");
    }

    [Fact]
    public void TryAddStock_InvalidStock_FalseAndNotAdded()
    {
        // Arrange
        var exchange = new GlobalBeverageCorporationExchange
        {
            Stocks = new Dictionary<string, Stock>()
        };
        var stock = new Stock { Symbol = "", VolumeWeightedPrice = 100.0m };
        const Int32 expectedNumberOfStocks = 0;

        // Act
        var actualResult = _stockService.TryAddStock(ref exchange, stock);

        // Assert
        actualResult.Should().BeFalse();
        exchange.Stocks.Count.Should().Be(expectedNumberOfStocks);
    }

    [Theory]
    [InlineData("A", 100.0)]
    [InlineData("B", 200.0)]
    public void TryAddStock_ValidStock_TrueAndAdded(string symbol, Decimal volumeWeightedPrice)
    {
        // Arrange
        var exchange = new GlobalBeverageCorporationExchange
        {
            Stocks = new Dictionary<string, Stock>()
        };
        var stock = new Stock { Symbol = symbol, VolumeWeightedPrice = volumeWeightedPrice };
        const Int32 expectedNUmberOfStocks = 1;

        // Act
        var actualResult = _stockService.TryAddStock(ref exchange, stock);

        // Assert
        actualResult.Should().BeTrue();
        exchange.Stocks.Count.Should().Be(expectedNUmberOfStocks);
        exchange.Stocks[symbol].Should().BeEquivalentTo(stock);
    }

    [Fact]
    public void TryAddStock_DuplicateStock_FalseAndNotAdded()
    {
        // Arrange
        var stock = new Stock { Symbol = "A", VolumeWeightedPrice = 100.0m };
        var exchange = new GlobalBeverageCorporationExchange
        {
            Stocks = new Dictionary<string, Stock>
            {
                { stock.Symbol, stock }
            }
        };
        var duplicateStock = new Stock { Symbol = "A", VolumeWeightedPrice = 200.0m };
        const Int32 expectedNUmberOfStocks = 1;

        // Act
        var actualResult = _stockService.TryAddStock(ref exchange, duplicateStock);

        // Assert
        actualResult.Should().BeFalse();
        exchange.Stocks.Count.Should().Be(expectedNUmberOfStocks);
        exchange.Stocks[stock.Symbol].Should().BeEquivalentTo(stock);
    }

    #endregion TryAddStock() tests
}
