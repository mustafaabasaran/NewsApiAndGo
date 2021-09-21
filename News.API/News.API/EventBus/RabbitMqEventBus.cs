using System;
using System.Net.Sockets;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using News.API.Model;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace News.API.EventBus
{
    public class RabbitMqEventBus : IEventBus
    {
        private IModel _consumerChannel;
        private string _queueName;
        
        const string BROKER_NAME = "newsapi_bus";
        
        private readonly IRabbitMQPersistentConnection _persistentConnection;
        private readonly ILogger<RabbitMqEventBus> _logger;
        private readonly int _retryCount;
        
        public RabbitMqEventBus(IRabbitMQPersistentConnection persistentConnection, ILogger<RabbitMqEventBus> logger, int retryCount)
        {
            _persistentConnection = persistentConnection;
            _logger = logger;
            _retryCount = retryCount;
        }

    
        public void Publish(NewsModel newsModel)
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            var policy = RetryPolicy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                {
                    _logger.LogWarning(ex, "Could not publish event: {Title} after {Timeout}s ({ExceptionMessage})", newsModel.Title, $"{time.TotalSeconds:n1}", ex.Message);
                });

            var eventName = newsModel.GetType().Name;

            _logger.LogTrace("Creating RabbitMQ channel to publish event: {Title} ({EventName})", newsModel.Title, eventName);

            using (var channel = _persistentConnection.CreateModel())
            {
                _logger.LogTrace("Declaring RabbitMQ exchange to publish event: {Title}", newsModel.Title);

                channel.QueueDeclare(queue: eventName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);
                                
                var body = JsonSerializer.SerializeToUtf8Bytes(newsModel, newsModel.GetType(), new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                policy.Execute(() =>
                {
                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    _logger.LogTrace("Publishing event to RabbitMQ: {Title}", newsModel.Title);

                    channel.BasicPublish(exchange: "",
                        routingKey: eventName,
                        basicProperties: properties,
                        body: body);
                });
            }
        }
        
    }
}