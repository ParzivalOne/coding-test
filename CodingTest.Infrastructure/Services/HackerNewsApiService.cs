using CodingTest.Domain.Constants;
using CodingTest.Domain.Interfaces;
using CodingTest.Domain.Models.Options;
using CodingTest.Domain.Models.Responses;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace CodingTest.Infrastructure.Services
{
    public class HackerNewsApiService(IHttpClientFactory httpClientFactory, IOptions<HackerNewsApiOptions> options, ILogger<HackerNewsApiService> logger) : IHackerNewsApiService
    {
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
        private readonly HackerNewsApiOptions _options = options.Value;
        private readonly ILogger<HackerNewsApiService> _logger = logger;

        public async Task<HackerNewsBestNewsResponse> GetBestNewsAsync()
        {
            var httpClient = _httpClientFactory.CreateClient(HttpClientConstants.HACKER_NEWS_CLIENT_NAME);
            try
            {
                var response = await httpClient.GetAsync(_options.BestNewsEndpoint);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var bestNewsIds = JsonSerializer.Deserialize<List<int>>(content, JsonSerializerOptions.Web) ?? new List<int>();

                return new()
                {
                    BestNewsIds = bestNewsIds
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching best news from {Endpoint}", _options.BestNewsEndpoint);
                throw;
            }
        }

        public async Task<HackerNewsGetByIdResponse> GetNewsById(int id)
        {
            var httpClient = _httpClientFactory.CreateClient(HttpClientConstants.HACKER_NEWS_CLIENT_NAME);
            try
            {
                var endpoint = _options.SpecificNewsEndpoint.Replace("{id}", id.ToString());
                var newsResponse = await httpClient.GetAsync(endpoint);
                newsResponse.EnsureSuccessStatusCode();
                var newsContent = await newsResponse.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<HackerNewsGetByIdResponse>(newsContent, JsonSerializerOptions.Web);
                if (result == null)
                {
                    _logger.LogWarning("Deserialized news item {Id} is null", id);
                    throw new InvalidOperationException($"Unable to deserialize news item {id}");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching news item {Id}", id);
                throw;
            }
        }
    }
}
