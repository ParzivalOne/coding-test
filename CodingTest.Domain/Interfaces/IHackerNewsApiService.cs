using CodingTest.Domain.Models.Responses;

namespace CodingTest.Domain.Interfaces
{
    public interface IHackerNewsApiService
    {
        Task<HackerNewsBestNewsResponse> GetBestNewsAsync();
        Task<HackerNewsGetByIdResponse> GetNewsById(int id);
    }
}