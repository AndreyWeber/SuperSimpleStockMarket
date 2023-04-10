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
    public async Task TryAddTrade_ValidTrade_Success()
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
        var actualResult = await _stockService.TryAddTradeAsync(stock, trade);

        // Assert
        actualResult.Should().BeTrue();
        stock.Trades.Should().NotBeEmpty();
        stock.Trades.Should().ContainKey(trade.TimeStamp);
        stock.Trades[trade.TimeStamp].Should().Be(trade);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("TEA")]
    public async Task TryAddTrade_InvalidTradeSymbol_Should_Failure(string tradeSymbol)
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
        var actualResult = await _stockService.TryAddTradeAsync(stock, trade);

        // Assert
        actualResult.Should().BeFalse();
        stock.Trades.Should().BeEmpty();
    }

    [Fact]
    public async Task TryAddTrade_NullStock_Throws_ArgumentNullException()
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
        var action = async () => await _stockService.TryAddTradeAsync(stock, trade);

        // Action
        await action.Should()
            .ThrowAsync<ArgumentNullException>()
            .WithMessage("*Argument cannot be null*");
    }

    [Fact]
    public async Task TryAddTrade_NullTrade_Throws_ArgumentNullException()
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
        var action = async () => await _stockService.TryAddTradeAsync(stock, trade);

        // Assert
        await action.Should()
            .ThrowAsync<ArgumentNullException>()
            .WithMessage("*Argument cannot be null*");
    }

    #endregion TryAddTrade() tests

    #region CalculateVolumeWeightedStockPrice() tests

    [Fact]
    public void CalculateVolumeWeightedStockPrice_NoTrades_ZeroResult()
    {
        // Arrange
        var stock = new Stock
        {
            Symbol = "TEST",
            Type = StockType.Common,
            LastDividend = 10.0m,
            Trades = new Dictionary<DateTime, Trade>()
        };
        const Decimal expectedVolumeWeightedStockPrice = Decimal.Zero;

        // Act
        var actualVolumeWeightedStockPrice = _stockService.CalculateVolumeWeightedStockPrice(ref stock);

        // Assert
        actualVolumeWeightedStockPrice.Should().Be(expectedVolumeWeightedStockPrice);
        stock.VolumeWeightedPrice.Should().Be(expectedVolumeWeightedStockPrice);
    }

    [Theory]
    [MemberData(nameof(GetTradesTestData))]
    public void CalculateVolumeWeightedStockPrice_WithTrades_Should_Succeed(
        Dictionary<DateTime, Trade> trades,
        Decimal expectedVolumeWeightedStockPrice,
        Decimal expectedPrecision
    )
    {
        // Arrange
        var stock = new Stock
        {
            Symbol = "TEST",
            Type = StockType.Common,
            LastDividend = 10.0m,
            Trades = trades
        };

        // Act
        var volumeWeightedStockPrice = _stockService.CalculateVolumeWeightedStockPrice(ref stock);

        // Assert
        volumeWeightedStockPrice.Should()
            .BeApproximately(expectedVolumeWeightedStockPrice, expectedPrecision);
        stock.VolumeWeightedPrice.Should()
            .BeApproximately(expectedVolumeWeightedStockPrice, expectedPrecision);
    }

    [Fact]
    public void CalculateVolumeWeightedStockPrice_NullStock_Throws_ArgumentNullException()
    {
        // Arrange
        Stock stock = null!;

        // Act
        var action = () => _stockService.CalculateVolumeWeightedStockPrice(ref stock);

        // Assert
        action.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("*Argument cannot be null*");
    }

    [Fact]
    public void CalculateVolumeWeightedStockPrice_ZeroQuantity_Throws_DivideByZeroException()
    {
        // Arrange
        var stock = new Stock
        {
            Symbol = "TEST",
            Type = StockType.Common,
            LastDividend = 10m,
            Trades = new Dictionary<DateTime, Trade>
            {
                {
                    DateTime.Now,
                    new Trade
                    {
                        Symbol = "TEST",
                        TimeStamp = DateTime.Now,
                        Quantity = 0,
                        Price = 50.0m,
                        Type = TradeType.Buy
                    }
                },
            }
        };

        // Act
        var action = () => _stockService.CalculateVolumeWeightedStockPrice(ref stock);

        // Assert
        action.Should()
            .Throw<DivideByZeroException>()
            .WithMessage("*has Quantity equal to zero*");
    }

    [Fact]
    public void CalculateVolumeWeightedStockPrice_Overflow_Throws_Exception()
    {
        // Arrange
        var stock = new Stock
        {
            Symbol = "TEST",
            Type = StockType.Common,
            LastDividend = 10.0m,
            Trades = new Dictionary<DateTime, Trade>
            {
                {
                    DateTime.Now,
                    new Trade
                    {
                        Symbol = "TEST",
                        TimeStamp = DateTime.Now,
                        Quantity = 1,
                        Price = Decimal.MaxValue,
                        Type = TradeType.Buy
                    }
                },
                {
                    DateTime.Now.AddSeconds(-1),
                    new Trade
                    {
                        Symbol = "TEST",
                        TimeStamp = DateTime.Now.AddSeconds(-1),
                        Quantity = 1,
                        Price = Decimal.MaxValue,
                        Type = TradeType.Buy
                    }
                },
            }
        };

        // Act
        var action = () => _stockService.CalculateVolumeWeightedStockPrice(ref stock);

        // Assert
        action.Should().Throw<Exception>().WithMessage("*Unexpected error occurred*");
    }

    public static IEnumerable<object[]> GetTradesTestData =>
        new List<object[]>
        {
            new object[]
            {
                new Dictionary<DateTime, Trade>
                {
                    {
                        DateTime.Now.AddSeconds(-10),
                        new Trade
                        {
                            Symbol = "TEST",
                            TimeStamp = DateTime.Now.AddSeconds(-10),
                            Quantity = 100,
                            Price = 50.0m,
                            Type = TradeType.Buy
                        }
                    },
                    {
                        DateTime.Now.AddSeconds(-20),
                        new Trade
                        {
                            Symbol = "TEST",
                            TimeStamp = DateTime.Now.AddSeconds(-20),
                            Quantity = 50,
                            Price = 40.0m,
                            Type = TradeType.Sell
                        }
                    },
                    {
                        DateTime.Now.AddSeconds(-30),
                        new Trade
                        {
                            Symbol = "TEST",
                            TimeStamp = DateTime.Now.AddSeconds(-30),
                            Quantity = 150,
                            Price = 60.0m,
                            Type = TradeType.Buy
                        }
                    },
                },
                53.3m, // expectedVolumeWeightedStockPrice
                0.33m // expectedPrecision
            }
        };

    #endregion CalculateVolumeWeightedStockPrice() tests
}