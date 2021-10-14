using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using TGS.GameProviders.KickPlayer.ReceiverServer.ReceiveHandlers;

namespace TGS.GameProviders.KickPlayer.ReceiverServer
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
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TGS.GameProviders.KickPlayer.ReceiverServer v1"));

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
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TGS.GameProviders.KickPlayerReceiver", Version = "v1" });
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
               .AddMongoRepository((options, sp) =>
               {
                   var config = sp.GetRequiredService<IConfiguration>();
                   config.GetSection("MongoDBConnectionString").Bind(options);
               });
            services
                .AddApiClients();
            services
                .AddCore();

            services
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
                                    queue: "gp.kickplayer.ky",
                                    durable: true,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);

                                channel.QueueBind(
                                    "gp.kickplayer.ky",
                                    "amq.fanout",
                                    $"gp.kickplayer.ky",
                                    new Dictionary<string, object>());

                                channel.QueueDeclare(
                                   queue: "gp.kickplayer.vr",
                                   durable: true,
                                   exclusive: false,
                                   autoDelete: false,
                                   arguments: null);

                                channel.QueueBind(
                                    "gp.kickplayer.vr",
                                    "amq.fanout",
                                    $"gp.kickplayer.vr",
                                    new Dictionary<string, object>());

                                channel.QueueDeclare(
                                  queue: "gp.kickplayer.wgvia",
                                  durable: true,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);

                                channel.QueueBind(
                                    "gp.kickplayer.wgvia",
                                    "amq.fanout",
                                    $"gp.kickplayer.wgvia",
                                    new Dictionary<string, object>());

                                channel.QueueDeclare(
                                  queue: "gp.kickplayer.wgfm",
                                  durable: true,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);

                                channel.QueueBind(
                                    "gp.kickplayer.wgfm",
                                    "amq.fanout",
                                    $"gp.kickplayer.wgfm",
                                    new Dictionary<string, object>());

                                channel.QueueDeclare(
                                  queue: "gp.kickplayer.biggaming",
                                  durable: true,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);

                                channel.QueueBind(
                                    "gp.kickplayer.biggaming",
                                    "amq.fanout",
                                    $"gp.kickplayer.biggaming",
                                    new Dictionary<string, object>());

                                channel.QueueDeclare(
                                 queue: "gp.kickplayer.tcg",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

                                channel.QueueBind(
                                    "gp.kickplayer.tcg",
                                    "amq.fanout",
                                    $"gp.kickplayer.tcg",
                                    new Dictionary<string, object>());
                            };
                        })
                       .AddHandler<KYKickPlayerReceiveHandler>("gp.kickplayer.ky", false)
                       .AddHandler<VRKickPlayerReceiveHandler>("gp.kickplayer.vr", false)
                       .AddHandler<WGHorseKickPlayerReceiveHandler>("gp.kickplayer.wgvia", false)
                       .AddHandler<WGSportsKickPlayerReceiveHandler>("gp.kickplayer.wgfm", false)
                       .AddHandler<BigGamingKickPlayerReceiveHandler>("gp.kickplayer.biggaming", false)
                       .AddHandler<TCGKickPlayerReceiveHandler>("gp.kickplayer.tcg", false);
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
                .AddTgsLog();
            services
                .AddKickPlayerCoreConfiguration();
            services
                .AddHealthChecks()
                .AddMongoDb(Configuration.GetSection("MongoDBConnectionString:GameProvider").Get<string>(), name: "MongoDB GameProvider")
                .AddRedis(Configuration.GetSection("RedisConnectionString:Default").Get<string>(), name: "Redis Default")
                .AddNpgSql(Configuration.GetConnectionString("Tgs"), name: "PostgreSQL Tgs");
            //.AddRabbitMQ(Configuration.GetSection("RabbitMQ:RabbitMQUrl").Get<string>(), name: "RabbitMQ Tgs");
        }
    }
}
