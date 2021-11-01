using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using TGS.GameProviders.GameRecord.ReceiverServer.ReceiveHandler;

namespace TGS.GameProviders.GameRecord.ReceiverServer
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
                endpoints.MapHealthChecks("/health", new()
                {
                    ResultStatusCodes = new Dictionary<HealthStatus, int>
                    {
                        {HealthStatus.Healthy, StatusCodes.Status200OK},
                        {HealthStatus.Unhealthy, StatusCodes.Status503ServiceUnavailable},
                        {HealthStatus.Degraded, StatusCodes.Status500InternalServerError}
                    },
                    ResponseWriter = async (context, report) =>
                    {
                        var hostName = Dns.GetHostName();
                        var hostEntry = await Dns.GetHostEntryAsync(hostName);
                        var hostIp = Array.Find(hostEntry.AddressList, x => x.AddressFamily == AddressFamily.InterNetwork);
                        var response = new
                        {
                            Data = new
                            {
                                MachineName = Environment.MachineName,
                                HostName = hostName,
                                HostIp = hostIp?.ToString() ?? string.Empty,
                                Environment = env.EnvironmentName,
                                OsVersion = $"{Environment.OSVersion}",
                                Version = Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                                ProcessCount = Environment.ProcessorCount,
                                AdditionalValues = new Dictionary<string, object>()
                            },
                            Result = new
                            {
                                Status = report.Status.ToString(),
                                TotalDuration = report.TotalDuration.TotalSeconds,
                                DependencyServices = report.Entries.Select(srv => new
                                {
                                    Key = srv.Key,
                                    Duration = srv.Value.Duration.TotalSeconds,
                                    Description = srv.Value.Description,
                                    Data = srv.Value.Data,
                                    Exception = srv.Value.Exception?.Message,
                                    Status = srv.Value.Status.ToString(),
                                    Tags = srv.Value.Tags.Any() ? string.Join(',', srv.Value.Tags) : string.Empty
                                }).ToArray()
                            }
                        };
                        var json = JsonSerializer.Serialize(response, new() { WriteIndented = true });
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(json);
                    }
                });
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
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TGS.GameProviders.GameRecord.ReceiverServer", Version = "v1" });
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

            services
                .AddMessageQueue()
                .AddRabbitMessageQueue(config =>
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

                                channel.QueueDeclare(
                                    queue: "gp.gamerecord.ly",
                                    durable: true,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);

                                channel.QueueBind(
                                    "gp.gamerecord.ly",
                                    "amq.direct",
                                    $"tgs.gp.gamerecord.ly",
                                    new Dictionary<string, object>());
                            };
                        })
                        .AddHandler<KYGameRecordReceiveHandler>("gp.gamerecord.ky", false)
                        .AddHandler<VRGameRecordReceiveHandler>("gp.gamerecord.vr", false)
                        .AddHandler<WGHorseGameRecordReceiveHandler>("gp.gamerecord.wgvia", false)
                        .AddHandler<WGSportsGameRecordReceiveHandler>("gp.gamerecord.wgfm", false)
                        .AddHandler<TCGGameRecordReceiveHandler>("gp.gamerecord.tcg", false)
                        .AddHandler<BigGamingFishingGameRecordReceiveHandler>("gp.gamerecord.biggaming.fishing", false)
                        .AddHandler<LYGameRecordReceiveHandler>("gp.gamerecord.ly", false);
                })
                .AddNatsMessageQueue(natsConfig =>
                {
                    natsConfig
                    .ConfigQueueOptions((options, sp) =>
                    {
                        var configuration = sp.GetRequiredService<IConfiguration>();
                        options.Url = configuration.GetSection("NatsMQ:Url").Get<string>();
                    });
                })
                .AddNatsGlobPatternExchange("*");

            services
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
            services
                .AddApiClients();
            services
                .AddGameProvidersCore();
            services
               .AddGameRecordCoreConfiguration((options, sp) =>
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
                .AddHealthChecks()
                .AddMongoDb(Configuration.GetSection("MongoDBConnectionString:GameProvider").Get<string>(), name: "MongoDB GameProvider")
                .AddRedis(Configuration.GetSection("RedisConnectionString:Default").Get<string>(), name: "Redis Default")
                .AddNpgSql(Configuration.GetConnectionString("Tgs"), name: "PostgreSQL Tgs");
            //.AddRabbitMQ(Configuration.GetSection("RabbitMQ:RabbitMQUrl").Get<string>(), name: "RabbitMQ Tgs");
        }
    }
}
