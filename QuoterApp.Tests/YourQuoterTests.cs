using AutoFixture;
using Moq;
using QuoterApp.MarketOrders;

namespace QuoterApp.Tests
{
    public class YourQuoterTests
    {
        IFixture _fixture;
        private readonly Mock<IMarketOrdersRepository> _marketOrdersRepositoryMock;
        private readonly YourQuoter _sut;

        public YourQuoterTests()
        {
            _fixture = new Fixture();
            _marketOrdersRepositoryMock = new Mock<IMarketOrdersRepository>();
            _sut = new YourQuoter(_marketOrdersRepositoryMock.Object);
        }

        [Fact]
        public void Instantiating_MarketOrdersRepositoryIsNull_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new YourQuoter(null));
        }

        [Fact]
        public void GetQuote_NoOrdersForInstrument_ThrowsArgumentException()
        {
            // Arrange
            var instrumentId = _fixture.Create<string>();
            var quantity = _fixture.Create<int>();
            _marketOrdersRepositoryMock.Setup(m => m.GetMarketOrders(instrumentId)).Returns(new List<MarketOrder>());

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _sut.GetQuote(instrumentId, quantity));
        }

        [Fact]
        public void GetQuote_NotEnoughQuantityForAllOrders_ThrowsArgumentException()
        {
            // Arrange
            var instrumentId = _fixture.Create<string>();
            var ordersForInstrument = _fixture.Build<MarketOrder>().With(mo=>mo.InstrumentId, instrumentId).CreateMany().ToList();     
            _marketOrdersRepositoryMock.Setup(m => m.GetMarketOrders(instrumentId)).Returns(ordersForInstrument);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _sut.GetQuote(instrumentId, int.MaxValue));
        }

        [Fact]
        public void GetQuote_ReturnsCorrectPriceForQuantity()
        {
            // Arrange
            var instrumentId = _fixture.Create<string>();
            var ordersForInstrument = new List<MarketOrder>
            {
                new MarketOrder
                {
                    InstrumentId = instrumentId,
                    Price = 101,
                    Quantity = 20
                },
                new MarketOrder
                {
                    InstrumentId = instrumentId,
                    Price = 100,
                    Quantity = 5
                }
            };
            _marketOrdersRepositoryMock.Setup(m => m.GetMarketOrders(instrumentId)).Returns(ordersForInstrument);

            // Act
            var result = _sut.GetQuote(instrumentId, 15);

            // Assert
            Assert.Equal(1510, result);
        }

        [Fact]
        public void GetVolumeWeightedAveragePrice_NoOrdersForInstrument_ThrowsArgumentException()
        {
            // Arrange
            var instrumentId = _fixture.Create<string>();
            _marketOrdersRepositoryMock.Setup(m => m.GetMarketOrders(instrumentId)).Returns(new List<MarketOrder>());

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _sut.GetVolumeWeightedAveragePrice(instrumentId));
        }

        [Fact]
        public void GetVolumeWeightedAveragePrice_ReturnsCorrectPrice()
        {
            // Arrange
            var instrumentId = _fixture.Create<string>();
            var ordersForInstrument = new List<MarketOrder>
            {
                new MarketOrder
                {
                    InstrumentId = instrumentId,
                    Price = 101,
                    Quantity = 20
                },
                new MarketOrder
                {
                    InstrumentId = instrumentId,
                    Price = 100,
                    Quantity = 5
                }
            };
            _marketOrdersRepositoryMock.Setup(m => m.GetMarketOrders(instrumentId)).Returns(ordersForInstrument);

            // Act
            var result = _sut.GetVolumeWeightedAveragePrice(instrumentId);

            // Assert
            Assert.Equal(100.8, result);
        }
    }
}
