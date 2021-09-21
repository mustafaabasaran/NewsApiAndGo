using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace News.API.Model
{
    public class NewsApiResult
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("totalResults")]
        public int TotalResults { get; set; }

        [JsonPropertyName("articles")]
        public List<ArticleModel> Articles { get; set; }
    }
}