using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using News.API.EventBus;
using News.API.Model;

namespace News.API.Commands
{
    public class NewsCommandHandler : IRequestHandler<NewsCommand, NewsApiResult>
    {
        private readonly ILogger<NewsCommandHandler> _logger;
        private readonly IHttpClientFactory _clientFactory;
        private readonly GoogleNewsApiSettings _googleNewsApiSettings;
        private readonly IEventBus _eventBus;
        private readonly IMapper _mapper;

        public NewsCommandHandler(ILogger<NewsCommandHandler> logger, IHttpClientFactory clientFactory, IOptions<GoogleNewsApiSettings> googleNewsApiSettings, IEventBus eventBus, IMapper mapper)
        {
            _logger = logger;
            _clientFactory = clientFactory;
            _eventBus = eventBus;
            _mapper = mapper;
            _googleNewsApiSettings = googleNewsApiSettings.Value;
        }
        
        public async Task<NewsApiResult> Handle(NewsCommand request, CancellationToken cancellationToken)
        {
            var result = new NewsApiResult();
            var query = GetGoogleNewsApiUrlWithQueryString(request);
            
            var googleApiRequest = new HttpRequestMessage(HttpMethod.Get, query);
            var client = _clientFactory.CreateClient();
            
            var response = await client.SendAsync(googleApiRequest, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);
                result = await JsonSerializer.DeserializeAsync<NewsApiResult>(responseStream, cancellationToken: cancellationToken);
            }
            else
            {
                _logger.LogError("News api call failed : " + response.StatusCode);
            }

            try
            {
                SendToQueue(result);
            }
            catch(Exception exception)
            {
                _logger.LogCritical($"Error occured when sending new to the queue. Error : {exception}");                
            }
            return result;
        }

        private void SendToQueue(NewsApiResult result)
        {
            if (result.Articles?.Count > 0)
            {
                foreach (ArticleModel item in result.Articles)
                {
                    NewsModel model = _mapper.Map<NewsModel>(item);
                    _eventBus.Publish(model);
                }
            }
        }

        private String GetGoogleNewsApiUrlWithQueryString(NewsCommand request)
        {
            var dict = new Dictionary<string, string>();
            dict.Add("q", request.Topic) ;
            dict.Add("sortBy", _googleNewsApiSettings.SortBy);
            dict.Add("from", request.BeginDate.ToString("yyyy-mm-dd"));
            dict.Add("to", request.EndDate.ToString("yyyy-mm-dd"));
            dict.Add("apiKey", _googleNewsApiSettings.ApiKey);
            return QueryHelpers.AddQueryString(_googleNewsApiSettings.Url , dict);
        }
        
         
    }
}