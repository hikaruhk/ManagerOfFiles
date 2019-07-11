using Client;
using Client.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrleansTests.Client
{
    [TestFixture, Category("BuildingUri")]
    public class ClientConfigurationExtensionsTests
    {
        [TestCase("abc", "https://google.com", "Test", "V1,V2", "Foos", Description = "Normal with parameters", ExpectedResult = "https://google.com/Foos?V1=R1&V2=R2")]
        [TestCase("abc", "https://google.com", "Test", "", "Foos", Description = "Normal without parameters", ExpectedResult = "https://google.com/Foos")]
        [TestCase("abc", "https://google.com", "Test", "", "", Description = "Normal without parameters or uri ext", ExpectedResult = "https://google.com/")]
        public string BuildUri(
            string apiKey,
            string baseURL,
            string name,
            string stringParam,
            string uriExt)
        {
            var configuration = new ClientConfiguration
            {
                ApiDataSources = Enumerable
                    .Range(0, 1)
                    .Select(s =>
                        new ApiDataSource
                        {
                            Name = "Bar",
                            ApiKey = apiKey,
                            BaseURL = baseURL,
                            EndPoints = new[]
                            {
                                new ApiEndPoint
                                {
                                    Name = name,
                                    Parameters = stringParam.Split(','),
                                    UriExtension = uriExt
                                }
                            }
                        }).ToDictionary(k => k.Name, v => v)
            };

            var result = configuration.BuildUri("Bar", "Test", "R1", "R2").ToString();

            return result;
        }

        [TestCase("abc", "https://google.com", "Test", "V1,V2,apiKey", "Foos", Description = "Normal with parameters", ExpectedResult = "https://google.com/Foos?V1=R1&V2=R2&apiKey=abc")]
        public string BuildUriWithApiKey(
            string apiKey,
            string baseURL,
            string name,
            string stringParam,
            string uriExt)
        {
            var configuration = new ClientConfiguration
            {
                ApiDataSources = Enumerable
                    .Range(0, 1)
                    .Select(s =>
                        new ApiDataSource
                        {
                            Name = "Bar",
                            ApiKey = apiKey,
                            BaseURL = baseURL,
                            EndPoints = new[]
                            {
                                new ApiEndPoint
                                {
                                    Name = name,
                                    Parameters = stringParam.Split(','),
                                    UriExtension = uriExt
                                }
                            }
                        }).ToDictionary(k => k.Name, v => v)
            };

            var result = configuration.BuildUriWithApiKey("Bar", "Test", "R1", "R2").ToString();

            return result;
        }

        [TestCase("bar", "Test", new[] { "R1", "R2" }, Description = "NonMatchingSource")]
        [TestCase("Bar", "Test", new[] { "R1", "R2" }, Description = "NonMatchingEndPoint")]
        public void BuildUriThatFails(
            string apiSource,
            string endPoint,
            string[] parameters)
        {
            var configuration = new ClientConfiguration
            {
                ApiDataSources = Enumerable
                    .Range(0, 1)
                    .Select(s =>
                        new ApiDataSource
                        {
                            Name = "Bar",
                            ApiKey = "abc",
                            BaseURL = "https://google.com",
                            EndPoints = new[]
                            {
                                new ApiEndPoint
                                {
                                    Name = "endpoint1",
                                    Parameters = new[] { "V1", "V2" },
                                    UriExtension = "Foos"
                                }
                            }
                        }).ToDictionary(k => k.Name, v => v)
            };

            Assert.Throws<UriFormatException>(
                () => configuration.BuildUriWithApiKey(
                    apiSource,
                    endPoint,
                    parameters));
        }
    }
}
