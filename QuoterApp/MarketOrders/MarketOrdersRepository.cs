using System.Collections.Generic;
using System.Linq;

namespace QuoterApp.MarketOrders
{
    public class MarketOrdersRepository : IMarketOrdersRepository
    {
        private List<MarketOrder> _marketOrders = new List<MarketOrder>();

        public void Add(MarketOrder marketOrder)
        {
            _marketOrders.Add(marketOrder);
        }

        public IEnumerable<MarketOrder> GetMarketOrders(string instrumentId)
        {
            return _marketOrders.Where(mo => mo.InstrumentId == instrumentId);
        }
    }
}
