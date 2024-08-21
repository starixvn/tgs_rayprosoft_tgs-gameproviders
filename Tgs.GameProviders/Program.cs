using System.Net;
using System.Net.Mime;
using System.Net.Sockets;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var filterInstrumentations = new[] { "html", "ico", "png", "jpg" };

var builder = WebApplication.CreateBuilder(args);

builder.Logging
	.AddOpenTelemetry(options =>
	{
	options.IncludeFormattedMessage = true;
	options.ParseStateValues = true;
		_ = options.AddFluentdForwardExporter(fluentOptions =>
		{
			var nodeIp = builder.Configuration.GetValue<string>("NODE_IP")!;
			fluentOptions.Host = nodeIp;
			fluentOptions.Tag = builder.Configuration.GetValue<string>("OTEL_SERVICE_NAME")!;
			fluentOptions.UseMessagePack();
		});
	});

// Add services to the container.
builder.Services
	.AddRouting(options => options.LowercaseUrls = true)
	.AddControllers(options =>
	{
		options.Filters.Add(new ProducesAttribute(MediaTypeNames.Application.Json));
		options.Filters.Add(new ConsumesAttribute(MediaTypeNames.Application.Json));
	})
	.AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	foreach (var xmlFileName in Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.xml"))
	{
		c.IncludeXmlComments(xmlFileName);
	}
});

builder.Services
	.AddGameProviders(
	(options, sp) =>
	{
		var configuration = sp.GetRequiredService<IConfiguration>();
		options.ConnectionString = configuration.GetConnectionString("Redis")!;
	},
	(options, sp) =>
	{
		var configuration = sp.GetRequiredService<IConfiguration>();
		options.IPAddress = builder.Configuration.GetSection("POD_IP").Value
		?? Dns.GetHostEntry(Dns.GetHostName()).AddressList.Where(ip => ip.AddressFamily == AddressFamily.InterNetwork).FirstOrDefault()?.ToString()
		?? "127.0.0.1";
	},
	(options, sp) =>
	{
		var configuration = sp.GetRequiredService<IConfiguration>();
		options.GameTokenUrl = configuration.GetValue<Uri>("GameProvidersConnectionStrings:GameTokenBaseUrl")!;
	},
	(options, sp) =>
	{
		var configuration = sp.GetRequiredService<IConfiguration>();
		options.BaseUrl = configuration.GetValue<Uri>("GameProvidersConnectionStrings:GameCommandClientBaseUrl")!;
	},
	(options, sp) =>
	{
		var configuration = sp.GetRequiredService<IConfiguration>();
		options.BaseUrl = configuration.GetValue<Uri>("GameProvidersConnectionStrings:GameRecordEndorphinaSessionBaseUrl")!;
	})
	.AddGameInterfaceAwcClient((options, sp) =>
	{
		var configuration = sp.GetRequiredService<IConfiguration>();
		options.BaseUrl = configuration.GetValue<Uri>("GameProvidersConnectionStrings:GameInterfaceAwcBaseUrl")!;
	})
	.AddGameInterfaceJiliClient((options, sp) =>
	{
		var configuration = sp.GetRequiredService<IConfiguration>();
		options.BaseUrl = configuration.GetValue<Uri>("GameProvidersConnectionStrings:GameInterfaceJiliBaseUrl")!;
	})
	.AddGameInterfaceWMCasinoServiceClient((options, sp) =>
	{
		var configuration = sp.GetRequiredService<IConfiguration>();
		options.BaseUrl = configuration.GetValue<Uri>("GameProvidersConnectionStrings:GameInterfaceWMCasinoBaseUrl")!;
	})
	.AddGameInterfaceEvoClient((options, sp) =>
	{
		var configuration = sp.GetRequiredService<IConfiguration>();
		options.BaseUrl = configuration.GetValue<Uri>("GameProvidersConnectionStrings:GameInterfaceEvolutionBaseUrl")!;
	})
	.AddGameInterfaceSkywindServiceClient((options, sp) =>
	{
		var configuration = sp.GetRequiredService<IConfiguration>();
		options.BaseUrl = configuration.GetValue<Uri>("GameProvidersConnectionStrings:GameInterfaceSkywindBaseUrl")!;
	})
	.AddGameInterfaceWGFmGamingServiceClient((options, sp) =>
	{
		var configuration = sp.GetRequiredService<IConfiguration>();
		options.BaseUrl = configuration.GetValue<Uri>("GameProvidersConnectionStrings:GameInterfaceWGFmGamingBaseUrl")!;
	})
	.AddGameInterfaceWGViaGamingClient((options, sp) =>
	{
		var configuration = sp.GetRequiredService<IConfiguration>();
		options.BaseUrl = configuration.GetValue<Uri>("GameProvidersConnectionStrings:GameInterfaceWGViaGamingBaseUrl")!;
	})
	.AddGameInterfaceJDBServiceClient((options, sp) =>
	{
		var configuration = sp.GetRequiredService<IConfiguration>();
		options.BaseUrl = configuration.GetValue<Uri>("GameProvidersConnectionStrings:GameInterfaceJDBBaseUrl")!;
	})
	.AddGameInterfaceSBOServiceClient((options, sp) =>
	{
		var configuration = sp.GetRequiredService<IConfiguration>();
		options.BaseUrl = configuration.GetValue<Uri>("GameProvidersConnectionStrings:GameInterfaceSBOBaseUrl")!;
	})
	.AddGameInterfaceBTiServiceClient((options, sp) =>
	{
		var configuration = sp.GetRequiredService<IConfiguration>();
		options.BaseUrl = configuration.GetValue<Uri>("GameProvidersConnectionStrings:GameInterfaceBTIBaseUrl")!;
	})
	.AddGameInterfaceHabaneroServiceClient((options, sp) =>
	{
		var configuration = sp.GetRequiredService<IConfiguration>();
		options.BaseUrl = configuration.GetValue<Uri>("GameProvidersConnectionStrings:GameInterfaceHabaneroBaseUrl")!;
	})
	.AddGameInterfaceSabaServiceClient((options, sp) =>
	{
		var configuration = sp.GetRequiredService<IConfiguration>();
		options.BaseUrl = configuration.GetValue<Uri>("GameProvidersConnectionStrings:GameInterfaceSabaBaseUrl")!;
	})
	.AddGameInterfaceSM01ServiceClient((options, sp) =>
	{
		var configuration = sp.GetRequiredService<IConfiguration>();
		options.BaseUrl = configuration.GetValue<Uri>("GameProvidersConnectionStrings:GameInterfaceSM01BaseUrl")!;
	})
	.AddGameInterfaceTFGamingServiceClient((options, sp) =>
	{
		var configuration = sp.GetRequiredService<IConfiguration>();
		options.BaseUrl = configuration.GetValue<Uri>("GameProvidersConnectionStrings:GameInterfaceTFGamingBaseUrl")!;
	})
	.AddGameInterfaceDreamGamingServiceClient((options, sp) =>
	{
		var configuration = sp.GetRequiredService<IConfiguration>();
		options.BaseUrl = configuration.GetValue<Uri>("GameProvidersConnectionStrings:GameInterfaceDreamGamingBaseUrl")!;
	})
	.AddGameInterfaceEndorphinaServiceClient((options, sp) =>
	{
		var configuration = sp.GetRequiredService<IConfiguration>();
		options.BaseUrl = configuration.GetValue<Uri>("GameProvidersConnectionStrings:GameInterfaceEndorphinaBaseUrl")!;
	})
	.AddGameInterfaceFaChaiClient((options, sp) =>
	{
		var configuration = sp.GetRequiredService<IConfiguration>();
		options.BaseUrl = configuration.GetValue<Uri>("GameProvidersConnectionStrings:GameInterfaceFaChaiBaseUrl")!;
	})
	.AddGameInterfaceOBetClient((options, sp) =>
	{
		var configuration = sp.GetRequiredService<IConfiguration>();
		options.BaseUrl = configuration.GetValue<Uri>("GameProvidersConnectionStrings:GameInterfaceOBetBaseUrl")!;
	})
	.AddGameInterfaceYeeBetClient((options, sp) =>
	{
		var configuration = sp.GetRequiredService<IConfiguration>();
		options.BaseUrl = configuration.GetValue<Uri>("GameProvidersConnectionStrings:GameInterfaceYeeBetBaseUrl")!;
	})
	.AddGameInterfacePragmaticPlayClient((options, sp) =>
	{
		var configuration = sp.GetRequiredService<IConfiguration>();
		options.BaseUrl = configuration.GetValue<Uri>("GameProvidersConnectionStrings:GameInterfacePragmaticPlayBaseUrl")!;
	})
	.AddGameInterfaceInplayMatrixClient((options, sp) =>
	{
		var configuration = sp.GetRequiredService<IConfiguration>();
		options.BaseUrl = configuration.GetValue<Uri>("GameProvidersConnectionStrings:GameInterfaceInplayMatrixBaseUrl")!;
	})
	.AddGameInterfaceCards365Client((options, sp) =>
	{
		var configuration = sp.GetRequiredService<IConfiguration>();
		options.BaseUrl = configuration.GetValue<Uri>("GameProvidersConnectionStrings:GameInterfaceCards365BaseUrl")!;
	})
	.AddGameInterfacePinnacleClient((options, sp) =>
	{
		var configuration = sp.GetRequiredService<IConfiguration>();
		options.BaseUrl = configuration.GetValue<Uri>("GameProvidersConnectionStrings:GameInterfacePinnacleBaseUrl")!;
	})
	.AddGameInterfaceJokerClient((options, sp) =>
	{
		var configuration = sp.GetRequiredService<IConfiguration>();
		options.BaseUrl = configuration.GetValue<Uri>("GameProvidersConnectionStrings:GameInterfaceJokerBaseUrl")!;
	})
	.AddGameInterfaceLEGamingClient((options, sp) =>
	{
		var configuration = sp.GetRequiredService<IConfiguration>();
		options.BaseUrl = configuration.GetValue<Uri>("GameProvidersConnectionStrings:GameInterfaceLEGamingBaseUrl")!;
	})
	.AddGameInterfaceKaiyuanClient((options, sp) =>
	{
		var configuration = sp.GetRequiredService<IConfiguration>();
		options.BaseUrl = configuration.GetValue<Uri>("GameProvidersConnectionStrings:GameInterfaceKaiyuanBaseUrl")!;
	})
	.AddGameInterfaceWicketCricketClient((options, sp) =>
	{
		var configuration = sp.GetRequiredService<IConfiguration>();
		options.BaseUrl = configuration.GetValue<Uri>("GameProvidersConnectionStrings:GameInterfaceWicketCricketBaseUrl")!;
	})
	.AddGameInterfaceGKXGamingClient((options, sp) =>
	{
		var configuration = sp.GetRequiredService<IConfiguration>();
		options.BaseUrl = configuration.GetValue<Uri>("GameProvidersConnectionStrings:GameInterfaceGKXGamingBaseUrl")!;
	})
	.AddGameInterfaceV8PokerClient((options, sp) =>
	{
		var configuration = sp.GetRequiredService<IConfiguration>();
		options.BaseUrl = configuration.GetValue<Uri>("GameProvidersConnectionStrings:GameInterfaceV8PokerBaseUrl")!;
	})
	.AddGameInterfaceCrPatiClient((options, sp) =>
	{
		var configuration = sp.GetRequiredService<IConfiguration>();
		options.BaseUrl = configuration.GetValue<Uri>("GameProvidersConnectionStrings:GameInterfaceCrPatiBaseUrl")!;
	})
	.AddGameInterfaceVRGamingClient((options, sp) =>
	{
		var configuration = sp.GetRequiredService<IConfiguration>();
		options.BaseUrl = configuration.GetValue<Uri>("GameProvidersConnectionStrings:GameInterfaceVRGamingBaseUrl")!;
	})
	.AddGameInterfaceFunkyClient((options, sp) =>
	{
		var configuration = sp.GetRequiredService<IConfiguration>();
		options.BaseUrl = configuration.GetValue<Uri>("GameProvidersConnectionStrings:GameInterfaceFunkyBaseUrl")!;
	})
	.AddGameInterfaceTPhereClient((options, sp) =>
	{
		var configuration = sp.GetRequiredService<IConfiguration>();
		options.BaseUrl = configuration.GetValue<Uri>("GameProvidersConnectionStrings:GameInterfaceTPhereBaseUrl")!;
	})
	.AddGameInterfaceCQ9GamingClient((options, sp) =>
	{
		var configuration = sp.GetRequiredService<IConfiguration>();
		options.BaseUrl = configuration.GetValue<Uri>("GameProvidersConnectionStrings:GameInterfaceCQ9GamingBaseUrl")!;
	})
	.AddGameInterfaceSharpSportsClient((options, sp) =>
	{
		var configuration = sp.GetRequiredService<IConfiguration>();
		options.BaseUrl = configuration.GetValue<Uri>("GameProvidersConnectionStrings:GameInterfaceSharpSportsBaseUrl")!;
	})
	.AddGameInterfaceGa28Client((options, sp) =>
	{
		var configuration = sp.GetRequiredService<IConfiguration>();
		options.BaseUrl = configuration.GetValue<Uri>("GameProvidersConnectionStrings:GameInterfaceGa28BaseUrl")!;
	})
	.AddGameInterfaceGlobalTechnologyClient((options, sp) =>
	{
		var configuration = sp.GetRequiredService<IConfiguration>();
		options.BaseUrl = configuration.GetValue<Uri>("GameProvidersConnectionStrings:GameInterfaceGlobalTechnologyBaseUrl")!;
	})
	.AddGameInterfaceMicroGamingClient((options, sp) =>
	{
		var configuration = sp.GetRequiredService<IConfiguration>();
		options.BaseUrl = configuration.GetValue<Uri>("GameProvidersConnectionStrings:GameInterfaceMicroGamingBaseUrl")!;
	})
	.AddGameInterfaceCRSportsClient((options, sp) =>
	{
		var configuration = sp.GetRequiredService<IConfiguration>();
		options.BaseUrl = configuration.GetValue<Uri>("GameProvidersConnectionStrings:GameInterfaceCRSportsBaseUrl")!;
	})
	.AddGameBetsClient(httpClientBuilder => httpClientBuilder.ConfigureHttpClient((sp, http) =>
	{
		var config = sp.GetRequiredService<IConfiguration>();
		http.BaseAddress = config.GetValue<Uri>("GameProvidersConnectionStrings:GameBetsClientBaseUrl");
	}));

builder.Services
	.AddOpenTelemetry()
	.ConfigureResource(_ => _.AddService(builder.Configuration.GetValue<string>("Otel:ServiceName")))
	.WithTracing(tracerProviderBuilder => tracerProviderBuilder
		.SetResourceBuilder(ResourceBuilder.CreateDefault()
			.AddEnvironmentVariableDetector())
		.AddSource("Tgs.*")
		.AddSource("TGS.*")
		.AddSource("Valhalla.*")
		.AddAspNetCoreInstrumentation(options => options.Filter =
			httpContext => !httpContext.Request.Path.HasValue
			|| !httpContext.Request.Path.StartsWithSegments("/swagger")
			&& !httpContext.Request.Path.StartsWithSegments("/health")
			&& !httpContext.Request.Path.StartsWithSegments("/profiler/")
			&& !httpContext.Request.Path.StartsWithSegments("/hangfire")
			&& !httpContext.Request.Path.StartsWithSegments("/metrics")
			&& !filterInstrumentations.Any(ext => httpContext.Request.Path.Value.EndsWith($".{ext}")))
		.AddHttpClientInstrumentation()
		.AddGrpcClientInstrumentation()
		.AddGrpcCoreInstrumentation()
		.AddEntityFrameworkCoreInstrumentation(options => options.SetDbStatementForText = true)
		.AddOtlpExporter(options => options.Endpoint = builder.Configuration.GetValue<Uri>("Otel:Collector")))
	.WithMetrics(meterBuiler => meterBuiler
	.SetResourceBuilder(ResourceBuilder.CreateDefault()
		.AddEnvironmentVariableDetector())
		.AddRuntimeInstrumentation()
		.AddAspNetCoreInstrumentation()
		.AddOtlpExporter())
	.Services
	.AddInterceptionService()
	.AddServiceUtiltiyInterceptors();

builder.Services.AddHealthChecks();

var app = builder.Build();

_ = app.UseSwagger();
_ = app.UseSwaggerUI();

_ = app.UseHttpsRedirection();

_ = app.UseAuthorization();

_ = app.MapControllers();

app.MapHealthChecks("/health");
app.MapHealthChecks("/");

_ = app.InitialMemory();

app.Run();
