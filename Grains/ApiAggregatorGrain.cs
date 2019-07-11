using Contracts.HTTP;
using Orleans;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Grains
{
    public interface IApiAggregatorGrain
    {
        Task<IEnumerable<string>> GetResults();
    }

    public class ApiAggregatorGrain : Grain, IApiAggregatorGrain
    {
        private IEnumerable<string> _payload;

        public override async Task OnActivateAsync()
        {
            var pk = this.GetPrimaryKeyString();
            


            RegisterTimer(
                GetContent,
                "",
                TimeSpan.FromMinutes(1),
                TimeSpan.FromMinutes(1));

            await base.OnActivateAsync();
        }

        private async Task<string> GetContent(object url)
        {
            return await Task.FromResult("");
        }

        public Task<IEnumerable<string>> GetResults()
        {
            throw new NotImplementedException();
        }
    }
}
