using NUnit.Framework;
using System.Threading.Tasks;
using Grains;

namespace OrleansTests
{
    [TestFixture, Category("SilosAndGrains")]
    public class ApiDataReaderGrainTests : InitializeSiloIntegration
    {
        private const string _url = "http://www.google.com";

        [Test]
        public async Task ApiDataReaderGrainReturningValue()
        {
            var grain = await Silo.CreateGrainAsync<ApiDataReaderGrain>(_url);
            var result = await grain.GetResult();

            Assert.That(HttpResultValue, Is.EqualTo(result));
        }
    }
}
