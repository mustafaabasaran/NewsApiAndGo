using System;
using System.Text.Json.Serialization;

namespace News.API.Model
{
    [Serializable]
    public class ArticleModel
    {
        [JsonPropertyName("source")]
        public SourceModel Source { get; set; }
        
        [JsonPropertyName("author")]
        public string Author { get; set; }
        
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
        
        [JsonPropertyName("url")]
        public string Url { get; set; }
        
        [JsonPropertyName("urlToImage")]
        public string UrlToImage { get; set; }
        
        [JsonPropertyName("publishedAt")]
        public DateTime PublishedAt { get; set; }
        
        [JsonPropertyName("content")]
        public string Content { get; set; }
    }
}