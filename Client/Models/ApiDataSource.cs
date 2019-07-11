namespace Client.Models
{
    public class ApiDataSource
    {
        public string Name { get; set; }
        public string ApiKey { get; set; }
        public string BaseURL { get; set; }
        public ApiEndPoint[] EndPoints { get;set; }
    }
}
