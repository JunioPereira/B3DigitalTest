using AutoFixture;
using B3DigitalModel;
using B3DigitalService;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using NUnit.Framework;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace B3DigitalTestNUnitTest.Service
{
    public class CalculateBestPriceTest
    {
        private Mock<IDistributedCache> MockIDistributedCache { get; set; }
        private Fixture Fixture { get; set; }

        [SetUp]
        public void Setup() 
        {
            MockIDistributedCache = new Mock<IDistributedCache>();
            Fixture = new Fixture();
        }

        [Test]
        public async Task AddValueTest() 
        {
            //Construindo os fakes
            var type = Fixture.Create<CriptoType>();
            var data = Fixture.Create<BidAsk>();

            ICalculateBestPrice iService = new CalculateBestPrice(MockIDistributedCache.Object);


            iService.AddValues(type, data);
        }

        [Test]
        public async Task CalculatePriceTest()
        {
            //Construindo os fakes
            var type = Fixture.Create<CriptoType>();
            var side = Fixture.Create<Side>();
            var quantity = Fixture.Create<int>();

            var data = Fixture.Build<BidAsk>()
                        .With(x => x.Symbol, type.ToString())
                        .Create();


            ICalculateBestPrice iService = new CalculateBestPrice(MockIDistributedCache.Object);

            //Inserindo os dados de mock
            iService.AddValues(type, data);


            iService.CalculatePrice(type, side, quantity);
        }
    }
}
