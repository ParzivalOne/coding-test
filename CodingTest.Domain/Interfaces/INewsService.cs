using CodingTest.Domain.Models.Responses;

namespace CodingTest.Domain.Interfaces
{
    public interface INewsService
    {
        Task<List<HackerNewsGetByIdResponse>> GetBestNewsAsync(int storyNumber);
    }
}