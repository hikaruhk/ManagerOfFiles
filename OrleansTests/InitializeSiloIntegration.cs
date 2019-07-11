using NUnit.Framework;
using System.Threading.Tasks;
using Orleans.TestKit;
using Moq;
using System.Net.Http;
using Moq.Protected;
using System.Threading;
using System.Net;

namespace OrleansTests
{
    [TestFixture, SetUpFixture]
    public abstract class InitializeSiloIntegration : TestKitBase
    {
        protected string HttpResultValue { get; } = "ResultValue";

        [OneTimeSetUp]
        public void Init()
        {
            var clientFactory = new Mock<IHttpClientFactory>();
            var httpClient = CreateMockedHttpClient(HttpStatusCode.OK, HttpResultValue);

            clientFactory.Setup(s => s.CreateClient(It.IsAny<string>()))
             .Returns(httpClient);

            Silo.AddService(clientFactory.Object);
        }

        private HttpClient CreateMockedHttpClient(
            HttpStatusCode statusCode,
            string message)
        {
            var messageHandler = new Mock<HttpMessageHandler>();
            messageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(
                    new HttpResponseMessage
                    {
                        StatusCode = statusCode,
                        Content = new StringContent(message)
                    });

            return new HttpClient(messageHandler.Object);
        }
    }
}
