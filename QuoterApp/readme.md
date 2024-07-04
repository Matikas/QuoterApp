QuoterApp is a console application that uses the MarketQuoter component to get quotes and calculate volume-weighted average price for instruments.

To read market orders *MarketOrderSourceReaderService* is used. It has a background job that keeps reading market orders throught out a day and adds them to repository for later use. For resiliency, it uses Polly to retry reading market orders in case of failure, it also logs error that occured.

*YourQuoter* uses a repository which is being populated by *MarketOrderSourceReaderService* to get market orders and calculates the best price and VWAP for an instrument. When getting best price (calling *GetQuote()*) it first takes order with the lowest price and if requested quantity is not available, it takes the next order with the lowest price and so on. There are also few edge cases that are handled:
- If there are no orders for an instrument, throws *ArgumentException*
- If requested quantity is greater than available quantity, throws *ArgumentException*

When reading VWAP (calling *GetVolumeWeightedAveragePrice()*) it first gets all orders for an instrument and then calculates VWAP. If there are no orders for an instrument, throws *ArgumentException*.

Unit tests are written for *YourQuoter* and *MarketOrderSourceReaderService* to cover all edge cases and to test if the logic is correct. *MarketOrdersRepository* is not tested because it is just a wrapper around *List<MarketOrder>* so in reality we would use some kind of database to store orders.



Original README.md:

Market Quoter Component

Your task is to implement market quoter component defined in *IQuoter* interface.

You have two methods to implement in *YourQuoter.cs*:
1. GetQuote
  - Takes instrument and quantity to quote, returns best possible price with current quotes
2. GetVolumeWeightedAveragePrice
  - Takes instrument id and calculates volume-weighted average price for the instrument
  - More about: https://en.wikipedia.org/wiki/Volume-weighted_average_price

*IMarketOrderSource.cs*
You should depend on IMarketOrderSource interface as stand-in for market data feed. Keep in mind that IMarketOrderSource.GetNextMarketOrder() blocks your call until next order is available, source is potentially endless.
You can use provided implementation of IQuoteSource as example or you can write your own.
You should not change IMarketOrderSource.cs in a significant way

*MarketOrder.cs*
Each individual market order is represented in MarketOrder class and has InstrumentId, Quantity at available at Price.
There can be many market orders for the same instrumnet with different quantities and different prices.

*Implementation Notes*
- Given implementation HardcodedQuoteSource of IMarketOrderSource simulated situation where there is limited number of orders available
- You are welcome to make your own implementation of IMarketOrderSource interface
- Consider implementation of IMarketOrderSource where it would be giving orders as they are created troughout the day in an open market
- For more advanced cases consider decoupling flow of getting orders from flow of getting quotes
- You are welcome to add (or not) any test that you see fit
