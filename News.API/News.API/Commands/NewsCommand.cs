using System;
using MediatR;
using News.API.Model;

namespace News.API.Commands
{
    [Serializable]
    public class NewsCommand : IRequest<NewsApiResult>
    {
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Topic { get; set; }
    }
}