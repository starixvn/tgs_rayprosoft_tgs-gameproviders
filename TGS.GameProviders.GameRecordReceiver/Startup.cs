using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using TGS.GameProviders.GameRecordReceiver.ReceiveHandler;

namespace TGS.GameProviders.GameRecordReceiver
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TGS.GameProviders.GameRecordReceiver v1"));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(configure =>
            {
                configure.Filters.Add<ExceptionFilter>();
            });
            services.AddSwaggerGen(c =>
            {
                string[] xmlFileNames = null;
                xmlFileNames = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.xml");
                foreach (var xmlFileName in xmlFileNames)
                {
                    c.IncludeXmlComments(xmlFileName);
                }
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TGS.GameProviders.GameRecordReceiver", Version = "v1" });
            });
            services.AddDbTgsRepository(
                    (sp, dbBuilder) =>
                    {
                        var config = sp.GetRequiredService<IConfiguration>();

                        dbBuilder
                            .UseLazyLoadingProxies();

                        dbBuilder.UseNpgsql(
                            config.GetConnectionString("Tgs"),
                            serverOption =>
                            {
                                serverOption.SetPostgresVersion(12, 5);
                                serverOption.CommandTimeout(180);
                                serverOption.UseTrigrams();
                            });

                        dbBuilder.UseSnakeCaseNamingConvention();
                    });
            services.AddDbBetRepository(
                    (sp, dbBuilder) =>
                    {
                        var config = sp.GetRequiredService<IConfiguration>();

                        dbBuilder
                            .UseLazyLoadingProxies();

                        dbBuilder.UseNpgsql(
                            config.GetConnectionString("Tgs"),
                            serverOption =>
                            {
                                serverOption.SetPostgresVersion(12, 5);
                                serverOption.CommandTimeout(180);
                                serverOption.UseTrigrams();
                            });

                        dbBuilder.UseSnakeCaseNamingConvention();
                    });
            services.AddRabbitMessageQueue(config =>
            {
                config
                    .ConfigQueueOptions((options, sp) =>
                    {
                        var configuration = sp.GetRequiredService<IConfiguration>();

                        configuration.GetSection("RabbitMQ").Bind(options);

                        options.OnSetupQueueAndExchange = channel =>
                        {
                            channel.QueueDeclare(
                                queue: "gp.gamerecord.ky",
                                durable: true,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);

                            channel.QueueBind(
                                "gp.gamerecord.ky",
                                "amq.direct",
                                $"tgs.gp.gamerecord.ky",
                                new Dictionary<string, object>());

                            channel.QueueDeclare(
                                queue: "gp.gamerecord.vr",
                                durable: true,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);

                            channel.QueueBind(
                                "gp.gamerecord.vr",
                                "amq.direct",
                                $"tgs.gp.gamerecord.vr",
                                new Dictionary<string, object>());

                            channel.QueueDeclare(
                                queue: "gp.gamerecord.wgvia",
                                durable: true,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);

                            channel.QueueBind(
                                "gp.gamerecord.wgvia",
                                "amq.direct",
                                $"tgs.gp.gamerecord.wgvia",
                                new Dictionary<string, object>());

                            channel.QueueDeclare(
                                queue: "gp.gamerecord.wgfm",
                                durable: true,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);

                            channel.QueueBind(
                                "gp.gamerecord.wgfm",
                                "amq.direct",
                                $"tgs.gp.gamerecord.wgfm",
                                new Dictionary<string, object>());

                            channel.QueueDeclare(
                                queue: "gp.gamerecord.tcg",
                                durable: true,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);

                            channel.QueueBind(
                                "gp.gamerecord.tcg",
                                "amq.direct",
                                $"tgs.gp.gamerecord.tcg",
                                new Dictionary<string, object>());

                            channel.QueueDeclare(
                                queue: "gp.gamerecord.biggaming.fishing",
                                durable: true,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);

                            channel.QueueBind(
                                "gp.gamerecord.biggaming.fishing",
                                "amq.direct",
                                $"tgs.gp.gamerecord.biggaming.fishing",
                                new Dictionary<string, object>());
                        };
                    })
                    .AddHandler<KYGameRecordReceiveHandler>("gp.gamerecord.ky", false)
                    .AddHandler<VRGameRecordReceiveHandler>("gp.gamerecord.vr", false)
                    .AddHandler<WGHorseGameRecordReceiveHandler>("gp.gamerecord.wgvia", false)
                    .AddHandler<WGSportsGameRecordReceiveHandler>("gp.gamerecord.wgfm", false)
                    .AddHandler<TCGGameRecordReceiveHandler>("gp.gamerecord.tcg", false)
                    .AddHandler<BigGamingFishingGameRecordReceiveHandler>("gp.gamerecord.biggaming.fishing", false);
            }).AddNatsMessageQueue(natsConfig =>
            {
                natsConfig
                .ConfigQueueOptions((options, sp) =>
                {
                    var configuration = sp.GetRequiredService<IConfiguration>();
                    options.Url = configuration.GetSection("NatsMQ:Url").Get<string>();
                });
            })
            .AddTgsLog();
            services
                .AddMongoRepository((options, sp) =>
                {
                    var configuration = sp.GetRequiredService<IConfiguration>();
                    configuration.GetSection("MongoDBConnectionString").Bind(options);
                });
            services
              .AddRedisRepository((options, sp) =>
              {
                  var configuration = sp.GetRequiredService<IConfiguration>();
                  configuration.GetSection("RedisConnectionString").Bind(options);
              });
            services.AddTCGFtpServiceClient((options, sp) =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                configuration.GetSection("TCGFtpApi").Bind(options);

                options.JsonSerializerOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    Converters =
                        {
                            new JsonStringEnumConverter()
                        }
                };
            });
            services
                .AddHttpClient()
                .AddApiClients();
            services
                .AddCore()
                .AddGameRecordCoreConfiguration();
        }
    }
}
