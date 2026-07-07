namespace CodingTest.Domain.Models.Responses
{
    public class NewsResponse
    {
        public string Title { get; set; }
        public string Uri { get; set; }
        public string PostedBy { get; set; }
        public DateTime Time { get; set; }
        public int Score { get; set; }
        public int CommentCount { get; set; }

        public NewsResponse() { }
        public NewsResponse(HackerNewsGetByIdResponse news)
        {
            Title = news.Title;
            Uri = news.Url;
            PostedBy = news.By;
            Time = DateTimeOffset.FromUnixTimeSeconds(news.Time).DateTime;
            Score = news.Score;
            CommentCount = news.Descendants;
        }
    }
}
