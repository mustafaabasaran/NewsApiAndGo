using System;

namespace News.API.Model
{
    public class NewsModel
    {
        public string SourceName { get; set; }
        public string Author { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}