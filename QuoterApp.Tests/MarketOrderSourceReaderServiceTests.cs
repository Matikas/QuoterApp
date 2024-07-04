using AutoFixture;
using Moq;
using Polly;
using QuoterApp.MarketOrders;

namespace QuoterApp.Tests
{
    public class MarketOrderSourceReaderServiceTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IMarketOrderSource> _marketOrderSourceMock;
        private readonly Mock<IMarketOrdersRepository> _marketOrdersRepositoryMock;
        private readonly Policy _noOpPolicy = Policy.NoOp();
        private readonly MarketOrderSourceReaderService _sut;

        public MarketOrderSourceReaderServiceTests()
        {
            _fixture = new Fixture();
            _marketOrderSourceMock = new Mock<IMarketOrderSource>();
            _marketOrdersRepositoryMock = new Mock<IMarketOrdersRepository>();
            _sut = new MarketOrderSourceReaderService(_marketOrderSourceMock.Object, _marketOrdersRepositoryMock.Object, _noOpPolicy);
        }

        [Fact]
        public void Instantiating_MarketOrdersRepositoryIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new MarketOrderSourceReaderService(null, _marketOrdersRepositoryMock.Object, _noOpPolicy));
        }

        [Fact]
        public void Instantiating_MarketOrderSourceIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new MarketOrderSourceReaderService(_marketOrderSourceMock.Object, null, _noOpPolicy));
        }

        [Fact]
        public void Instantiating_ReadingMarketOrdersPolicyIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new MarketOrderSourceReaderService(_marketOrderSourceMock.Object, _marketOrdersRepositoryMock.Object, null));
        }

        [Fact]
        public async Task Starting_StartsGettingMarketOrdersFromMarketOrderSource()
        {
            // Act
            _sut.Start();
            await Task.Delay(10).ConfigureAwait(false); // Wait for the background worker to start

            // Assert
            _marketOrderSourceMock.Verify(m => m.GetNextMarketOrder(), Times.AtLeastOnce);
        }

        [Fact]
        public async Task Starting_WhenAlreadyStarted_ThrowsInvalidOperationException()
        {
            // Arrange
            _sut.Start();
            await Task.Delay(10).ConfigureAwait(false); // Wait for the background worker to start

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _sut.Start());
        }

        [Fact]
        public async Task Starting_AddsReceivedMarketOrdersToMarketOrdersRepository()
        {
            // Arrange
            var marketOrder = _fixture.Create<MarketOrder>();
            _marketOrderSourceMock.Setup(m => m.GetNextMarketOrder()).Returns(marketOrder);

            // Act
            _sut.Start();
            await Task.Delay(10).ConfigureAwait(false); // Wait for the background worker to start

            // Assert
            _marketOrdersRepositoryMock.Verify(m => m.Add(marketOrder));
        }
    }
}