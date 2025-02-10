namespace HackerNews.Domain.Entities
{
    public class StoryDto
    {
        public string Title { get; set; }
        public string Uri { get; set; }
        public string PostedBy { get; set; }
        public string Time { get; set; }
        public int Score { get; set; }
        public int CommentCount { get; set; }

        public StoryDto(Story story)
        {
            Title = story.Title;
            Uri = story.Uri;
            PostedBy = story.PostedBy;
            Time = DateTimeOffset.FromUnixTimeSeconds(story.UnixTime).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss+00:00");
            Score = story.Score;
            CommentCount = story.Kids?.Count ?? 0;
        }
    }
}
