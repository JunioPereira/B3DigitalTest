using AutoFixture;
using B3DigitalModel;
using B3DigitalTest.Controller;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Text;

namespace B3DigitalTestNUnitTest.WebApi.Controller.V1
{
    public class RedisFindControllerTests
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
        public async Task GetStateTest()
        {
            //Construindo os fakes
            var type = Fixture.Create<CriptoType>();
            CancellationTokenSource cancellationToken = new CancellationTokenSource();
            cancellationToken.CancelAfter(10000);

            //dados de retorno
            var quoteInfo = Fixture.Build<QuoteInfo>()
                                .With(x => x.Symbol, type.ToString())
                                .Create();

            var obj = JsonConvert.SerializeObject(quoteInfo);
            var body = Encoding.UTF8.GetBytes(obj);

            MockIDistributedCache.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).
                Returns(Task.FromResult(body));

            RedisFindController controller = new RedisFindController(MockIDistributedCache.Object);

            var result = await controller.GetState(type, cancellationToken.Token);
            var contentesult = result as OkObjectResult;

            contentesult?.Should().NotBeNull();
            contentesult?.Value.Should().BeEquivalentTo(quoteInfo);
        }

        [Test]
        public async Task GetCalculoTest()
        {
            //Construindo os fakes
            var id = Fixture.Create<Guid>();
            CancellationTokenSource cancellationToken = new CancellationTokenSource();
            cancellationToken.CancelAfter(10000);

            //dados de retorno
            var data = Fixture.Build<BestPricePayload>()
                                .With(x => x.Id, id)
                                .Create();

            var obj = JsonConvert.SerializeObject(data);
            var body = Encoding.UTF8.GetBytes(obj);

            MockIDistributedCache.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).
                Returns(Task.FromResult(body));

            RedisFindController controller = new RedisFindController(MockIDistributedCache.Object);

            var result = await controller.GetCalculo(id, cancellationToken.Token);
            var contentesult = result as OkObjectResult;

            contentesult?.Should().NotBeNull();
            contentesult?.Value.Should().BeEquivalentTo(data);
        }
    }
}
