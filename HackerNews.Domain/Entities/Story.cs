using System.Text.Json.Serialization;

namespace HackerNews.Domain.Entities
{
    public class Story
    {
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("url")]
        public string Uri { get; set; } = string.Empty;

        [JsonPropertyName("by")]
        public string PostedBy { get; set; } = string.Empty;

        [JsonPropertyName("time")]
        public long UnixTime { get; set; }

        [JsonPropertyName("score")]
        public int Score { get; set; } = 0;

        [JsonPropertyName("kids")]
        public List<int> Kids { get; set; } = new List<int>();
    }
}
