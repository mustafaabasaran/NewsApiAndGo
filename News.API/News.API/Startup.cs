using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using News.API.Behaviours;
using News.API.EventBus;
using News.API.Model;
using RabbitMQ.Client;

namespace News.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddHttpClient();
            services.Configure<GoogleNewsApiSettings>(Configuration.GetSection("GoogleNewsApiSettings"));
            
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddMediatR(Assembly.GetExecutingAssembly());
            AddRabbitMqServices(services);
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo {Title = "News.API", Version = "v1"}); });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "News.API v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }


        private void AddRabbitMqServices(IServiceCollection services)
        {
            services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();

                var factory = new ConnectionFactory()
                {
                    HostName = Configuration["RabbitMqSettings:ConnectionUrl"],
                    DispatchConsumersAsync = true
                };

                if (!string.IsNullOrEmpty(Configuration["RabbitMqSettings:Username"]))
                {
                    factory.UserName = Configuration["RabbitMqSettings:Username"];
                }

                if (!string.IsNullOrEmpty(Configuration["RabbitMqSettings:Password"]))
                {
                    factory.Password = Configuration["RabbitMqSettings:Password"];
                }

                var retryCount = 5;
                if (!string.IsNullOrEmpty(Configuration["RabbitMqSettings:RetryCount"]))
                {
                    retryCount = int.Parse(Configuration["RabbitMqSettings:RetryCount"]);
                }

                return new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
            });
            
            services.AddSingleton<IEventBus, RabbitMqEventBus>(sp =>
            {
                var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
                var logger = sp.GetRequiredService<ILogger<RabbitMqEventBus>>();

                var retryCount = 5;
                if (!string.IsNullOrEmpty(Configuration["RabbitMqSettings:RetryCount"]))
                {
                    retryCount = int.Parse(Configuration["RabbitMqSettings:RetryCount"]);
                }

                return new RabbitMqEventBus(rabbitMQPersistentConnection, logger, retryCount);
            });
        }
    }
}