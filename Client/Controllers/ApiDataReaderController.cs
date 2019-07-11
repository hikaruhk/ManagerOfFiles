using Client.Models;
using Contracts.HTTP;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Orleans;
using System.Threading.Tasks;

namespace Client.Controllers
{
    [Produces("application/json")]
    [Route("api/dataloader")]
    public class ApiDataReaderController : Controller
    {
        private IClusterClient _orleansClient;
        private ClientConfiguration _clientConfiguration;

        public ApiDataReaderController(
            IClusterClient orleansClient,
            IOptionsMonitor<ClientConfiguration> options)
        {
            _clientConfiguration = options.CurrentValue;
            _orleansClient = orleansClient;
        }

        [HttpGet("dailytimeseries")]
        public Task<string> GetDailyTimeSeries(string symbol)
        {
            var url = _clientConfiguration.BuildUriWithApiKey("Alphavantage", "TimeSeries", symbol);
            var grain = _orleansClient.GetGrain<IApiDataReaderGrain>(url.ToString());

            return grain.GetResult();
        }
    }
}
