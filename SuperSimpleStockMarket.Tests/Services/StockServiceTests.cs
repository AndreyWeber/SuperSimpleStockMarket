using SuperSimpleStockMarket.Services;
using SuperSimpleStockMarket.Services.Interfaces;
using SuperSimpleStockMarket.Models;
using Microsoft.Extensions.Logging;
using FluentAssertions;
using Moq;

namespace SuperSimpleStockMarket.Tests;

public class StockServiceTests
{
    private readonly IStockService _stockService;
    private readonly ILogger<StockService> _logger;

    public StockServiceTests()
    {
        _logger = new Mock<ILogger<StockService>>().Object;
        _stockService = new StockService(_logger);
    }

    #region CalculateDividendYield() tests

    [Theory]
    [MemberData(nameof(CalculateDividendYield_GetTestData))]
    public void CalculateDividendYield_ValidArgs_Should_Succeed(
        StockType stockType,
        Decimal? fixedDividend,
        Decimal price,
        Decimal expectedDididendYield
    )
    {
        // Arrange
        var stock = new Stock
        {
            Symbol = "TEST",
            Type = stockType, //StockType.Common,
            FixedDividend = (Decimal?) fixedDividend,
            LastDividend = 10m,
            ParValue = 100m
        };

        // Act
        var actualDividendYield = _stockService.CalculateDividendYield(ref stock, price);

        // Assert
        expectedDididendYield.Should().Be(actualDividendYield);
        expectedDididendYield.Should().Be(stock.DividendYield);
    }

    [Fact]
    public void CalculateDividendYield_NullStock_Throws_ArgumentNullException()
    {
        // Arrange
        Stock stock = null!;
        Decimal price = 50.0m;

        // Act
        var action = () => _stockService.CalculateDividendYield(ref stock!, price);

        // Assert
        action.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("*Argument cannot be null*");
    }

    public static IEnumerable<object[]> CalculateDividendYield_GetTestData()
    {
        // Each row:
        //   stockType
        //   fixedDividend
        //   price
        //   expectedDididendYield
        yield return new Object[] { StockType.Common, null!, 50.0m, 0.2m };
        yield return new Object[] { StockType.Common, Decimal.Zero, 50.0m, 0.2m };
        yield return new Object[] { StockType.Common, 0.05m, 50.0m, 0.2m };
        yield return new Object[] { StockType.Common, null!, Decimal.Zero, Decimal.Zero };
        yield return new Object[] { StockType.Common, Decimal.Zero, Decimal.Zero, Decimal.Zero };
        yield return new Object[] { StockType.Common, 0.05m, Decimal.Zero, Decimal.Zero };
        yield return new Object[] { StockType.Preferred, null!, Decimal.Zero, Decimal.Zero };
        yield return new Object[] { StockType.Preferred, Decimal.Zero, Decimal.Zero, Decimal.Zero };
        yield return new Object[] { StockType.Preferred, 0.05m, Decimal.Zero, Decimal.Zero };
        yield return new Object[] { StockType.Preferred, null!, 50.0m, Decimal.Zero };
        yield return new Object[] { StockType.Preferred, Decimal.Zero, 50.0m, Decimal.Zero };
        yield return new Object[] { StockType.Preferred, 0.05m, 50.0m, 0.1m };
    }

    #endregion CalculateDividendYield() tests

    #region CalculatePERatio() tests

    [Theory]
    [InlineData(0.0, 0.0, 0.0)]
    [InlineData(50.0, 0.0, 0.0)]
    [InlineData(0.0, 0.2, 0.0)]
    [InlineData(50.0, 0.2, 250.0)]
    public void CalculatePERatio_ValidArgs_Should_Succeed(
        Decimal price,
        Decimal dividendYield,
        Decimal expectedPERatio
    )
    {
        // Arrange
        var stock = new Stock
        {
            Symbol = "TEST",
            DividendYield = dividendYield
        };

        // Act
        var actualPERatio = _stockService.CalculatePERatio(ref stock, price);

        // Assert
        expectedPERatio.Should().Be(actualPERatio);
        expectedPERatio.Should().Be(stock.PERatio);
    }

    [Fact]
    public void CalculatePERatio_NullStock_Throws_ArgumentNullException()
    {
        // Arrange
        Stock stock = null!;
        var price = 50.0m;

        // Act
        var action = () => _stockService.CalculatePERatio(ref stock, price);

        // Assert
        action.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("*Argument cannot be null*");
    }

    #endregion CalculatePERatio() tests

    #region TryAddTrade() tests

    [Fact]
    public void TryAddTrade_ValidTrade_Success()
    {
        // Arrange
        var stock = new Stock
        {
            Symbol = "TEST",
            Type = StockType.Common,
            LastDividend = 10.0m
        };
        var trade = new Trade
        {
            Symbol = "TEST",
            TimeStamp = DateTime.Now,
            Quantity = 100,
            Price = 50.0m,
            Type = TradeType.Buy
        };

        // Act
        var actualResult = _stockService.TryAddTrade(ref stock, trade);

        // Assert
        actualResult.Should().BeTrue();
        stock.Trades.Should().NotBeEmpty();
        stock.Trades.Should().ContainKey(trade.TimeStamp);
        stock.Trades[trade.TimeStamp].Should().Be(trade);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void TryAddTrade_InvalidTradeSymbol_Should_Failure(string tradeSymbol)
    {
        // Arrange
        var stock = new Stock
        {
            Symbol = "TEST",
            Type = StockType.Common,
            LastDividend = 10.0m
        };
        var trade = new Trade
        {
            Symbol = tradeSymbol,
            TimeStamp = DateTime.Now,
            Quantity = 100,
            Price = 50.0m,
            Type = TradeType.Buy
        };

        // Act
        var actualResult = _stockService.TryAddTrade(ref stock, trade);

        // Assert
        actualResult.Should().BeFalse();
        stock.Trades.Should().BeEmpty();
    }

    [Fact]
    public void TryAddTrade_NullStock_Throws_ArgumentNullException()
    {
        // Arrange
        Stock stock = null!;
        var trade = new Trade
        {
            Symbol = "TEST",
            TimeStamp = DateTime.Now,
            Quantity = 100,
            Price = 50.0m,
            Type = TradeType.Buy
        };

        // Act
        var action = () => _stockService.TryAddTrade(ref stock, trade);

        // Action
        action.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("*Argument cannot be null*");
    }

    [Fact]
    public void TryAddTrade_NullTrade_Throws_ArgumentNullException()
    {
        // Arrange
        var stock = new Stock
        {
            Symbol = "TEST",
            Type = StockType.Common,
            LastDividend = 10.0m
        };
        Trade trade = null!;

        // Act
        var action = () => _stockService.TryAddTrade(ref stock, trade);

        // Assert
        action.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("*Argument cannot be null*");
    }

    #endregion TryAddTrade() tests

    #region CalculateVolumeWeightedStockPrice() tests



    #endregion CalculateVolumeWeightedStockPrice() tests
}