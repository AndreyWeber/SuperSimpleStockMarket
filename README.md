# Super Simple Stock Market
## Requirements
1. The Global Beverage Corporation Exchange is a new stock market trading in drinks companies.
   * Your company is building an object-oriented system to run that trading.
   * You have been assigned to build part of the core object model for a limited phase 1
2. Provide the complete source code that will:
   * For a given stock,
     * Given any price as input, calculate the dividend yield
     * Given any price as input,  calculate the P/E Ratio
     * Record a trade, with timestamp, quantity, buy or sell indicator and price
     * Calculate Volume Weighted Stock Price based on trades in past 5 minutes
   * Calculate the GBCE All Share Index using the geometric mean of the Volume Weighted Stock Price for all stocks
## Constraints & Notes
1. Written in one of these languages - Java, C#, C++, Python
2. The source code should be suitable for forming part of the object model of a production application, and can be proven to meet the requirements. A shell script is not an appropriate submission for this assignment.
3. No database, GUI or I/O is required, all data need only be held in memory
4. No prior knowledge of stock markets or trading is required – all formulas are provided below.
5. The code should provide only the functionality requested, however it must be production quality.
#### Table 1. Sample data from the Global Beverage Corporation Exchange

| Stock Symbols | Type      | Last Dividend | Fixed Dividend | Par Value |
| ------------- | --------- | -------------:| --------------:| ---------:|
| TEA           | Common    | 0             |                | 100       |
| POP           | Common    | 8             |                | 100       |
| ALE           | Common    | 23            |                | 60        |
| GIN           | Preferred | 8             | 2%             | 100       |
| JOE           | Common    | 12            |                | 250       |
#### Table 2. Formulas
<table>
  <thead>
    <tr>
      <th>&nbsp;</th>
      <th>Common</th>
      <th>Preferred</th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td><strong>Dividend Yield</strong></td>
      <td>LastDividend / Price</td>
      <td>(Fixed Dividend * Par Value) / Price</td>
    </tr>
    <tr>
      <td><strong>P/E Ratio</strong></td>
      <td colspan="2" align="center">Price / Dividend</td>
    </tr>
    <tr>
      <td><strong>Geometric Mean</strong></td>
      <td colspan="2" align="center"><sup>n</sup>&radic; p<sub>1</sub> * p<sub>2</sub> * p<sub>3</sub>* ... p<sub>n</sub></td>
    </tr>
    <tr>
      <td><strong>Volume Weighted Stock Price</strong></td>
      <td colspan="2" align="center">(&sum;<sub>i</sub> TradedPrice<sub>i</sub> * Quantity<sub>i</sub>) / &sum;<sub>i</sub> Quantity<sub>i</sub></td>
    </tr>
  </tbody>
</table>

## Implementation
### Summary
1. Solution implemented using C# 9.0, .NET Core 7.0
2. Unit test framework: xUnit 2.4.2, Moq 4.18.4, FluentAssertions 6.10.0
3. IDE: Visual Studio Code v1.77.1
4. Code documented with XML for C# documentation comments
### Solution Structure
- __SuperSimpleStockMarket__ - a library containing the implementation of all requirements listed above. This library is designed to be used as part of a core object model or registered in a DI container as a component of a RESTful Web API. All data models are implemented as plain data classes, ready for JSON serialization, and are decoupled from services that encapsulate model behavior and data processing. This ensures flexibility and maintainability.
- __SuperSimpleStockMarket.Api__ - sample Web API demostrating __SuperSimpleStockMarket__ usage.
- __SuperSimpleStockMarket.Tests__ - unit tests demonstrating that the __SuperSimpleStockMarket__ library meets the requirements.
### How To Run SuperSimpleStockMarket.Api
Open Visual Studio Code and choose one of the following options to run the API in debug mode:
   - Press F5
   - From the top menu: _Run_ -> _Start Debugging_
   - From the side menu: press _Run and Debug_ button -> select _.NET Core Launch (Web API)_ from a drop-down list on top -> press Play button
#### Web API
- URL: http://localhost:5177
- API documentation will be accessible via SwaggerUI on http://localhost:5177/api/docs/index.html. SwaggerUI can be used to test API methods
- [Postman](https://www.postman.com/) [collection](https://github.com/AndreyWeber/SuperSimpleStockMarket/blob/develop/SuperSimpleStockMarket.postman_collection.json) also can be used to tests API methods
### How To Run Unit Tests
1. Open the solution folder in Visual Studio Code and run the following Tasks using the top menu:
   - _Terminal_ -> _Run Task..._ -> _build_
   - _Terminal_ -> _Run Task..._ -> _test_
2. Alternatively, unit tests can be executed using the `dotnet` CLI:
   - Open a terminal window
   - Navigate to the __SuperSimpleStockMarket.Tests__ folder
   - Run the following command from the command line: `dotnet test`
