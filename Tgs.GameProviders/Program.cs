using System;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenTelemetry.Exporter;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Prometheus;
using Tgs.GameProviders;
using Tgs.GameProviders.ViewModelMappings;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("/config/appsettings.json", true)
    .AddEnvironmentVariables();

builder.Logging
    .AddOpenTelemetry(options =>
    {
        options.IncludeFormattedMessage = true;
        options.ParseStateValues = true;
        _ = options.AddFluentdForwardExporter(fluentOptions =>
        {
            var nodeIp = builder.Configuration.GetValue<string>("NODE_IP");
            fluentOptions.Host = nodeIp;
            fluentOptions.Tag = builder.Configuration.GetValue<string>("OTEL_SERVICE_NAME");
            fluentOptions.UseMessagePack();
        });
    });

// Add services to the container.

builder.Services
    .AddRouting(options => options.LowercaseUrls = true)
    .AddControllers(configure =>
    {
        _ = configure.Filters.Add<ExceptionFilter>();
    })
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services
    .AddSwaggerGen(c =>
    {
        var xmlFileNames = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.xml");
        foreach (var xmlFileName in xmlFileNames)
            c.IncludeXmlComments(xmlFileName);
    });

builder.Services
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

builder.Services
    .AddApiClients()
    .AddGameProvidersService((options, sp) =>
    {
        var configuration = sp.GetRequiredService<IConfiguration>();
        options.ConnectionString = configuration.GetConnectionString("Redis");
    },
    (options, sp) =>
    {
        var configuration = sp.GetRequiredService<IConfiguration>();
        options.BaseUrl = configuration.GetValue<Uri>("GameProviders:GameProvidersQueryBaseUrl");
    },
    (options, sp) =>
    {
        var configuration = sp.GetRequiredService<IConfiguration>();
        options.BaseUrl = configuration.GetValue<Uri>("GameProviders:GameInterfaceAwcBaseUrl");
    },
    (options, sp) =>
    {
        var configuration = sp.GetRequiredService<IConfiguration>();
        options.BaseUrl = configuration.GetValue<Uri>("GameProviders:GameInterfaceJiliBaseUrl");
    },
    (options, sp) =>
    {
        var configuration = sp.GetRequiredService<IConfiguration>();
        options.BaseUrl = configuration.GetValue<Uri>("GameProviders:GameInterfaceWMCasinoBaseUrl");
    },
    (options, sp) =>
    {
        var configuration = sp.GetRequiredService<IConfiguration>();
        options.Url = configuration.GetValue<Uri>("GameProviders:GameInterfaceEvolutionBaseUrl");
    },
    (options, sp) =>
    {
        var configuration = sp.GetRequiredService<IConfiguration>();
        options.BaseUrl = configuration.GetValue<Uri>("GameProviders:GameInterfaceIdnLiveBaseUrl");
    },
    (options, sp) =>
    {
        var configuration = sp.GetRequiredService<IConfiguration>();
        options.BaseUrl = configuration.GetValue<Uri>("GameProviders:GameRoundsBaseUrl");
    },
    (options, sp) =>
    {
        var configuration = sp.GetRequiredService<IConfiguration>();
        options.GameTokenUrl = configuration.GetValue<Uri>("GameProviders:GameTokenBaseUrl");
    },
    (options, sp) =>
    {
        var configuration = sp.GetRequiredService<IConfiguration>();
        options.BaseUrl = configuration.GetValue<Uri>("GameProviders:GamePlayerTransformBaseUrl");
    })
    .AddGameProvidersRepositories()
    .AddGameProvidersServices((sp) => sp.GetRequiredService<RedisConnectionFactory>().Database);

builder.Services
    .AddAutoMapper(config =>
    {
        config.AddProfile<GameUrlMapping>();
        config.AddProfile<PlayerFundMapping>();
        config.AddProfile<PlayerFundCheckingMapping>();
        config.AddProfile<RoundDetailMapping>();
    });

builder.Services
     .AddSingleton<IConfigureOptions<OtlpExporterOptions>>(
        sp => new ConfigureNamedOptions<OtlpExporterOptions>(
            Options.DefaultName,
            exporterOptions =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var nodeIp = config.GetValue<string>("NODE_IP");

                if (!string.IsNullOrWhiteSpace(nodeIp))
                    exporterOptions.Endpoint = new Uri($"http://{nodeIp}:4317");

                config.GetSection("Opentelemetry:Tracing").Bind(exporterOptions);
            }))
    .AddOpenTelemetryTracing(traceBuilder => traceBuilder
        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddEnvironmentVariableDetector())
        .AddSource("Tgs.*")
        .AddSource("TGS.*")
        .AddGrpcClientInstrumentation()
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddProfileViewExporter()
        .AddOtlpExporter())
    .AddProfileViewer()
    .Configure<AspNetCoreInstrumentationOptions>(options
        => options.Filter = httpContext
            => !httpContext.Request.Path.HasValue
                || !httpContext.Request.Path.StartsWithSegments("/swagger")
                && !httpContext.Request.Path.StartsWithSegments("/healthz")
                && !httpContext.Request.Path.StartsWithSegments("/profiler/")
                && !new[] { "html", "ico", "png", "jpg" }.Any(
                    ext => httpContext.Request.Path.Value.EndsWith($".{ext}")));

builder.Services
    .AddInterceptionService()
    .AddServiceUtiltiyInterceptors();

builder.Services
    .AddHealthChecks()
    .ForwardToPrometheus();

var app = builder.Build();

_ = app.UseSwagger();
_ = app.UseSwaggerUI();

_ = app.UseHttpsRedirection();

_ = app.UseAuthorization();

_ = app.MapControllers();

app.MapProfileViewer();

app.MapHealthChecks("/healthz");

app.Run();
