using Orleans;
using System.Threading.Tasks;

namespace Contracts.HTTP
{
    public interface IApiDataReaderGrain : IGrainWithStringKey
    {
        Task<string> GetResult();
    }
}