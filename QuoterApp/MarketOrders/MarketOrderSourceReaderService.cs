using Polly;
using System;
using System.ComponentModel;

namespace QuoterApp.MarketOrders
{
    public class MarketOrderSourceReaderService : IDisposable
    {
        private readonly IMarketOrderSource _marketOrderSource;
        private readonly IMarketOrdersRepository _marketOrdersRepository;
        private readonly BackgroundWorker _backgroundOrderSourceReader;
        private readonly Policy _readingMarketOrdersPolicy;

        public MarketOrderSourceReaderService(IMarketOrderSource marketOrderSource, IMarketOrdersRepository marketOrdersRepository, Policy readingMarketOrdersPolicy)
        {
            _marketOrderSource = marketOrderSource ?? throw new ArgumentNullException(nameof(marketOrderSource));
            _marketOrdersRepository = marketOrdersRepository ?? throw new ArgumentNullException(nameof(marketOrdersRepository));
            _readingMarketOrdersPolicy = readingMarketOrdersPolicy ?? throw new ArgumentNullException(nameof(readingMarketOrdersPolicy)); ;
            _backgroundOrderSourceReader = new BackgroundWorker();
            _backgroundOrderSourceReader.DoWork += BackgroundSourceReader_DoWork;
        }

        public void Start()
        {
            if (_backgroundOrderSourceReader.IsBusy)
            {
                throw new InvalidOperationException("Already started");
            }

            _backgroundOrderSourceReader.RunWorkerAsync();
        }

        private void BackgroundSourceReader_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                var newOrder = _readingMarketOrdersPolicy.Execute(_marketOrderSource.GetNextMarketOrder);
                _marketOrdersRepository.Add(newOrder);
            }
        }

        public void Dispose()
        {
            // It is not mandatory to dispose BackgroundWorker because .Dispose() doesn't do anything - BackgroundWorker inherits Dispose() from base class Component
            // but doesn't actually do anything in it. Adding Dispose just in case if we switch to a different implementation of BackgroundWorker in the future that
            // we wouldn't forget to address possible memory leaks.
            _backgroundOrderSourceReader.Dispose();
        }
    }
}
