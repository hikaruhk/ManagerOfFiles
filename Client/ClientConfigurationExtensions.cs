using Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Client
{
    public static class ClientConfigurationExtensions
    {
        public static Uri BuildUriWithApiKey(
            this ClientConfiguration @clientConfig,
            string dataSource,
            string endPointName,
            params string[] inputParameters) => BuildUriInternal(@clientConfig, dataSource, endPointName, true, inputParameters);

        public static Uri BuildUri(
            this ClientConfiguration @clientConfig,
            string dataSource,
            string endPointName,
            params string[] inputParameters) => BuildUriInternal(@clientConfig, dataSource, endPointName, false, inputParameters);

        private static Uri BuildUriInternal(
            this ClientConfiguration @clientConfig,
            string dataSource,
            string endPointName,
            bool includeApiKey,
            params string[] inputParameters)
        {
            @clientConfig.ApiDataSources.TryGetValue(dataSource, out var clientSource);

            var endPoint = clientSource
                ?.EndPoints
                ?.SingleOrDefault(f => f.Name == endPointName);

            var updatedInputParams = includeApiKey
                ? inputParameters.Append(clientSource?.ApiKey)
                : inputParameters;

            return clientSource == default(ApiDataSource) || endPoint == default(ApiEndPoint)
                ? throw new UriFormatException("Unable to construct an uri given the parameters")
                : ConstructUri(clientSource, endPoint, updatedInputParams).Uri;
        }

        private static UriBuilder ConstructUri(
            ApiDataSource apiDataSource, 
            ApiEndPoint apiEndPoint,
            IEnumerable<string> inputParameters)
        {
            var parameters = apiEndPoint
                .Parameters
                .Where(w => !string.IsNullOrWhiteSpace(w))
                .Zip(inputParameters, (a, b) => $"{a}={b}")
                .DefaultIfEmpty(string.Empty)
                .Aggregate((a, b) => $"{a}&{b}");

            return new UriBuilder(new Uri(apiDataSource.BaseURL))
            {
                Path = apiEndPoint.UriExtension,
                Query = parameters,
            };
        }
    }
}
