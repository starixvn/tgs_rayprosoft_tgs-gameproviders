using System;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Exporter;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Prometheus;
using Tgs.GameProviders.Repositories.DependencyInjection;
using Tgs.GameProviders.ViewModelMappings;

namespace Tgs.GameProviders
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
            => Configuration = configuration;

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            _ = app.UseSwagger();
            _ = app.UseSwaggerUI();

            _ = app.UseHttpsRedirection();

            _ = app.UseRouting();

            _ = app.UseAuthorization();

            _ = app.UseEndpoints(endpoints =>
            {
                _ = endpoints.MapControllers();
                _ = endpoints.MapHealthChecks("/healthz");
                _ = endpoints.MapMetrics();
                endpoints.MapProfileViewer();
            });
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            _ = services
                .AddRouting(options => options.LowercaseUrls = true)
                .AddControllers(configure =>
                {
                    _ = configure.Filters.Add<ExceptionFilter>();
                })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            _ = services.AddSwaggerGen(c =>
            {
                var xmlFileNames = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.xml");
                foreach (var xmlFileName in xmlFileNames) c.IncludeXmlComments(xmlFileName);
            });

            _ = services
                .AddMessageQueue()
                .AddNatsMessageQueue(natsConfig =>
                {
                    _ = natsConfig
                    .ConfigQueueOptions((options, sp) =>
                    {
                        var configuration = sp.GetRequiredService<IConfiguration>();
                        options.Url = configuration.GetConnectionString("Nats");
                    });
                })
                .AddNatsGlobPatternExchange("*");

            _ = services
                .AddApiClients()
                .UseRedis((options, sp) =>
                {
                    var configuration = sp.GetRequiredService<IConfiguration>();
                    options.ConnectionString = configuration.GetConnectionString("Redis");
                })
                .AddGameProvidersRepositories()
                .AddGameProvidersServices((sp) => sp.GetRequiredService<RedisConnectionFactory>().Database)
                .AddGameProviders((options, sp) =>
                {
                    var configuration = sp.GetRequiredService<IConfiguration>();
                    options.BaseUrl = configuration.GetValue<Uri>("GameProviders:GameProvidersQueryServerBaseUrl");
                },
                (options, sp) =>
                {
                    var configuration = sp.GetRequiredService<IConfiguration>();
                    options.BaseUrl = configuration.GetValue<Uri>("GameProviders:GameInterfaceAwcServiceBaseUrl");
                },
                (options, sp) =>
                {
                    var configuration = sp.GetRequiredService<IConfiguration>();
                    options.BaseUrl = configuration.GetValue<Uri>("GameProviders:GameInterfaceJiliServiceBaseUrl");
                },
                (options, sp) =>
                {
                    var configuration = sp.GetRequiredService<IConfiguration>();
                    options.BaseUrl = configuration.GetValue<Uri>("GameProviders:GameInterfaceWMCasinoServiceBaseUrl");
                },
                (options, sp) =>
                {
                    var configuration = sp.GetRequiredService<IConfiguration>();
                    options.BaseUrl = configuration.GetValue<Uri>("GameProviders:GameRoundsServiceBaseUrl");
                });

            _ = services
                .AddAutoMapper(config =>
                {
                    config.AddProfile<GameUrlMapping>();
                    config.AddProfile<PlayerFundMapping>();
                    config.AddProfile<PlayerFundCheckingMapping>();
                    config.AddProfile<RoundDetailMapping>();
                });

            _ = services
                .AddOptions<OtlpExporterOptions>()
                .Configure<IConfiguration>((options, conifg) =>
                {
                    var nodeIp = conifg.GetValue<string>("NODE_IP");

                    if (!string.IsNullOrWhiteSpace(nodeIp))
                        options.Endpoint = new Uri($"http://{nodeIp}:4317");

                    conifg.GetSection("Opentelemetry:Tracing").Bind(options);
                });

            _ = services.AddOpenTelemetryTracing(builder =>
            {
                _ = builder
                    .SetResourceBuilder(ResourceBuilder.CreateDefault()
                    .AddEnvironmentVariableDetector())
                    .AddSource("Tgs.*")
                    .AddSource("TGS.*")
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation(options => options.SetDbStatementForText = true)
                    .AddProfileViewExporter()
                    .AddOtlpExporter();
            });

            _ = services.Configure<AspNetCoreInstrumentationOptions>(options =>
            {
                options.Filter = httpContext =>
                {
                    return !httpContext.Request.Path.HasValue
                        || !httpContext.Request.Path.StartsWithSegments("/swagger")
                        && !httpContext.Request.Path.StartsWithSegments("/healthz")
                        && !httpContext.Request.Path.StartsWithSegments("/profiler/")
                        && !httpContext.Request.Path.StartsWithSegments("/metrics")
                        && !new[] { "html", "ico", "png", "jpg", "js" }.Any(
                            ext => httpContext.Request.Path.Value.EndsWith($".{ext}"));
                };
            });

            _ = services.AddProfileViewer();

            _ = services
                .AddInterceptionService()
                .AddServiceUtiltiyInterceptors();

            _ = services
                .AddHealthChecks()
                .ForwardToPrometheus();
        }
    }
}
