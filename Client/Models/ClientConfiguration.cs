using System.Collections.Generic;

namespace Client.Models
{
    public class ClientConfiguration
    {
        public IDictionary<string, ApiDataSource> ApiDataSources { get; set; }
    }
}
