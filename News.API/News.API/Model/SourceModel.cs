using System;
using System.Text.Json.Serialization;

namespace News.API.Model
{
    [Serializable]
    public class SourceModel
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}