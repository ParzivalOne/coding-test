using CodingTest.Domain.Models.Responses;

namespace CodingTest.Domain.Interfaces
{
    public interface INewsService
    {
        Task<BestNNewsResponse> GetBestNewsAsync(int storyNumber);
    }
}