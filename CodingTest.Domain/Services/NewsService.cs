using CodingTest.Domain.Interfaces;
using CodingTest.Domain.Models.Options;
using CodingTest.Domain.Models.Responses;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CodingTest.Domain.Services
{
    public class NewsService(IHackerNewsApiService hackerNewsApiService, IMemoryCache cache, IOptions<CacheOptions> options, ILogger<NewsService> logger) : INewsService
    {
        private readonly IHackerNewsApiService _hackerNewsApiService = hackerNewsApiService;
        private readonly IMemoryCache _cache = cache;
        private readonly CacheOptions _options = options.Value;
        private readonly ILogger<NewsService> _logger = logger;

        private const string BEST_NEWS_CACHE_KEY = "BestNewsIds";
        private const string SPECIFIC_NEWS_CACHE_KEY_PREFIX = "SpecificNews_";
        private const int MAX_CONCURRENCY = 8;

        public async Task<List<HackerNewsGetByIdResponse>> GetBestNewsAsync(int storyNumber)
        {
            var bestNewsResponse = await _cache.GetOrCreateAsync(BEST_NEWS_CACHE_KEY, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_options.BestNewsExpirationInSeconds);
                try
                {
                    return await _hackerNewsApiService.GetBestNewsAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to retrieve best news ids from HackerNews API");
                    throw;
                }
            });

            var ids = bestNewsResponse?.BestNewsIds ?? new List<int>();

            var semaphore = new SemaphoreSlim(MAX_CONCURRENCY);
            var fetchTasks = ids.Select(async id =>
            {
                await semaphore.WaitAsync();
                try
                {
                    try
                    {
                        var item = await _cache.GetOrCreateAsync(SPECIFIC_NEWS_CACHE_KEY_PREFIX + id, async entry =>
                        {
                            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_options.SpecificNewsExpirationInSeconds);
                            return await _hackerNewsApiService.GetNewsById(id);
                        });

                        return item;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to fetch news item {Id}", id);
                        return null;
                    }
                }
                finally
                {
                    semaphore.Release();
                }
            }).ToList();

            var results = await Task.WhenAll(fetchTasks);
            var newsList = results.Where(x => x != null).ToList();

            return newsList.OrderByDescending(n => n.Score).Take(storyNumber).ToList();
        }
    }
}
