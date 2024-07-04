using QuoterApp.MarketOrders;
using System;
using System.Linq;

namespace QuoterApp
{
    public class YourQuoter : IQuoter
    {
        private readonly IMarketOrdersRepository _marketOrdersRepository;

        public YourQuoter(IMarketOrdersRepository marketOrdersRepository)
        {
            _marketOrdersRepository = marketOrdersRepository ?? throw new ArgumentNullException(nameof(marketOrdersRepository));
        }

        public double GetQuote(string instrumentId, int quantity)
        {
            var ordersForInstrument = _marketOrdersRepository.GetMarketOrders(instrumentId);

            if (!ordersForInstrument.Any())
            {
                throw new ArgumentException($"No orders for instrument {instrumentId}");
            }

            var quantityToQuote = quantity;
            var priceForQuantity = 0.0;
            foreach (var order in ordersForInstrument.OrderBy(o => o.Price))
            {
                var quantityFromOrder = quantityToQuote < order.Quantity ? quantityToQuote : order.Quantity;
                quantityToQuote -= quantityFromOrder;
                priceForQuantity += quantityFromOrder * order.Price;

                if (quantityToQuote <= 0)
                {
                    return priceForQuantity;
                }
            }

            throw new ArgumentException($"Not enough quantity for all orders for instrument {instrumentId}");
        }

        public double GetVolumeWeightedAveragePrice(string instrumentId)
        {
            var ordersForInstrument = _marketOrdersRepository.GetMarketOrders(instrumentId);

            if (!ordersForInstrument.Any())
            {
                throw new ArgumentException($"No orders for instrument {instrumentId}");
            }

            return ordersForInstrument.Sum(o => o.Price * o.Quantity) / ordersForInstrument.Sum(o => o.Quantity);
        }
    }
}
