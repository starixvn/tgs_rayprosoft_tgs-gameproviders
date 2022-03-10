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
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Npgsql;
using OpenTelemetry.Exporter;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Prometheus;
using TGS.GameProviders.Core.Extensions;
using TGS.GameProviders.Microsoft.Extensions.DependencyInjection;
using TGS.GameProviders.ViewModelMappings;

namespace TGS.GameProviders
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(
                    IConfiguration configuration)
            => Configuration = configuration;

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //使用IP解析需要使用
            app.UseForwardedHeaders();

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TGS.GameProviders v1"));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/healthz");
                endpoints.MapMetrics();
                endpoints.MapProfileViewer();
            });
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(
            IServiceCollection services)
        {
            services
                .AddRouting(options => options.LowercaseUrls = true)
                .AddControllers(configure =>
                {
                    configure.Filters.Add<ExceptionFilter>();
                })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            services.AddSwaggerGen(c =>
            {
                var xmlFileNames = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.xml");
                foreach (var xmlFileName in xmlFileNames) c.IncludeXmlComments(xmlFileName);

                c.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Title = "TGS.GameProviders",
                        Version = "v1"
                    });
            });

            services
                .AddMessageQueue()
                .AddNatsMessageQueue(natsConfig =>
                {
                    natsConfig
                    .ConfigQueueOptions((options, sp) =>
                    {
                        var configuration = sp.GetRequiredService<IConfiguration>();
                        options.Url = configuration.GetConnectionString("NatsMQ");
                    });
                })
                .AddNatsGlobPatternExchange("*");

            services.AddOptions<ForwardedHeadersOptions>()
                .Configure<IConfiguration>((options, config) =>
                {
                    options.ForwardedHeaders = ForwardedHeaders.All;
                    options.ForwardLimit = config.GetValue<int>("ForwardedHeaders:ForwardLimit");

                    var knownProxies = config.GetSection("ForwardedHeaders:KnownProxies").Get<string[]>();
                    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;

                    // Replace with IP of your proxy/load balancer
                    foreach (var knownProxie in knownProxies)
                        options.KnownProxies.Add(IPAddress.Parse(knownProxie));

                    var knowNetworks = config.GetSection("ForwardedHeaders:KnowNetworks").Get<string[]>() ?? Array.Empty<string>();
                    foreach (var cidr in knowNetworks)
                    {
                        (string ip, string range) = cidr.Split("/");

                        options.KnownNetworks.Add(
                            new IPNetwork(
                                IPAddress.Parse(ip),
                                string.IsNullOrEmpty(range)
                                    ? 32
                                    : int.Parse(range)));
                    }
                });

            services
                .AddApiClients()
                .UseRedis((options, sp) =>
                {
                    var configuration = sp.GetRequiredService<IConfiguration>();
                    options.ConnectionString = configuration.GetConnectionString("RedisConfigurationString");
                })
                .AddGameProvidersServices((sp) => sp.GetRequiredService<RedisConnectionFactory>().Database)
                .AddGameProviders((options, sp) =>
                {
                    var configuration = sp.GetRequiredService<IConfiguration>();
                    options.BaseUrl = configuration.GetValue<Uri>("GameProviders:GameProvidersQueryServerBaseUrl");
                });

            services
                .AddAutoMapper(config =>
                {
                    config.AddProfile<GameUrlMapping>();
                    config.AddProfile<PlayerFundMapping>();
                    config.AddProfile<PlayerFundCheckingMapping>();
                    config.AddProfile<RoundDetailMapping>();
                });

            services
                .AddOptions<OtlpExporterOptions>()
                .Configure<IConfiguration>((options, conifg) =>
                {
                    var nodeIp = conifg.GetValue<string>("NODE_IP");

                    if (!string.IsNullOrWhiteSpace(nodeIp))
                        options.Endpoint = new Uri($"http://{nodeIp}:4317");

                    conifg.GetSection("Opentelemetry:Tracing").Bind(options);
                });

            services.AddOpenTelemetryTracing(builder =>
            {
                builder
                    .SetResourceBuilder(ResourceBuilder.CreateDefault()
                    .AddEnvironmentVariableDetector())
                    .AddSource("Tgs.*")
                    .AddSource("TGS.*")
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddNpgsql()
                    .AddAspNetCoreInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation(options => options.SetDbStatementForText = true)
                    .AddProfileViewExporter()
                    .AddOtlpExporter();
            });

            services.Configure<AspNetCoreInstrumentationOptions>(options =>
            {
                options.Filter = httpContext =>
                {
                    return !httpContext.Request.Path.HasValue
                        || !httpContext.Request.Path.StartsWithSegments("/swagger")
                        && !httpContext.Request.Path.StartsWithSegments("/healthz")
                        && !httpContext.Request.Path.StartsWithSegments("/profiler/")
                        && !new[] { "html", "ico", "png", "jpg", "js" }.Any(
                            ext => httpContext.Request.Path.Value.EndsWith($".{ext}"));
                };
            });

            services.AddProfileViewer();

            services
                .AddInterceptionService()
                .AddServiceUtiltiyInterceptors();

            services
                .AddHealthChecks()
                .ForwardToPrometheus();
        }
    }
}
