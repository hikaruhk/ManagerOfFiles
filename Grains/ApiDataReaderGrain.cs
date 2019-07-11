using Contracts.HTTP;
using Orleans;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Grains
{
    public class ApiDataReaderGrain : Grain, IApiDataReaderGrain
    {
        private string _payload;
        private IHttpClientFactory _clientFactory;

        public ApiDataReaderGrain(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public override async Task OnActivateAsync()
        {
            var url = this.GetPrimaryKeyString();
            _payload = await GetStringAsync(url);

            RegisterTimer(
                GetStringAsync,
                url,
                TimeSpan.FromMinutes(1),
                TimeSpan.FromMinutes(1));

            await base.OnActivateAsync();
        }

        public Task<string> GetResult() => Task.FromResult(_payload);

        private async Task<string> GetStringAsync(object url)
        {
            var client = _clientFactory.CreateClient();
            var response = await client.GetAsync(url as string);

            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
    }
}
