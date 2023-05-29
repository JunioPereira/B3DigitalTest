using AutoFixture;
using B3DigitalModel;
using B3DigitalService;
using B3DigitalTest.Controller;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace B3DigitalTestNUnitTest.WebApi.Controller.V1
{
    public class BestPriceControllerTests
    {
        private Mock<ICalculateBestPrice> MockICalculateBestPrice { get; set; }
        private Fixture Fixture { get; set; }

        [SetUp]
        public void Setup()
        {
            MockICalculateBestPrice = new Mock<ICalculateBestPrice>();
            Fixture = new Fixture();
        }

        [Test]
        public async Task GetTest()
        {
            //Construindo os fakes
            var type = Fixture.Create<CriptoType>();
            var side = Fixture.Create<Side>();
            var quantity = Fixture.Create<int>();

            //dados de retorno
            var bestPricePayload = Fixture.Build<BestPricePayload>()
                                    .With(x => x.Side, side)
                                    .Create();

            MockICalculateBestPrice.Setup(x => x.CalculatePrice(It.IsAny<CriptoType>(), It.IsAny<Side>(), It.IsAny<int>())).
                Returns(bestPricePayload);

            BestPriceController controller = new BestPriceController(MockICalculateBestPrice.Object);

            var result = await controller.Get(type, side, quantity);
            var contentesult = result as OkObjectResult;

            contentesult?.Should().NotBeNull();
            contentesult?.Value.Should().BeEquivalentTo(bestPricePayload);
        }
    }
}
