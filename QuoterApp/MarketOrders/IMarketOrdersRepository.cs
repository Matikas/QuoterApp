using System;
using System.Collections.Generic;

namespace QuoterApp.MarketOrders
{
    public interface IMarketOrdersRepository
    {
        void Add(MarketOrder marketOrder);
        IEnumerable<MarketOrder> GetMarketOrders(string instrumentId);
    }
}
