using System.Text.Json.Nodes;
using NSubstitute;
using Tgs.GameCommand.Client;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Repositories.UnitTest;

public class GameProviderRepositoryUnitTest
{
    [Fact]
    public async Task Get_AVIAGaming_ConfigurationAsync()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();

        var gameProviderId = Guid.NewGuid();
        var gameProviderConfigurationId = Guid.NewGuid();

        var settingConfiguration = new AVIAGamingConfiguration()
        {
            AmountMaximum = 10000000,
            AmountFormat = "10,2",
            ApiUrl = new Uri("http://localhost/"),
            Authorization = "authorization",
            PlayerPassword = "TgsP@ssw0rd",
        };

        _ = fakeClient
            .SettingGetConfigurationAsync<AVIAGamingConfiguration>(Arg.Is(GameProviderConfigurations.AVIAGaming))
            .Returns(settingConfiguration);

        _ = fakeClient.GameProviderConfigurationGetAsync(gameProviderConfigurationId).Returns(new GameProviderConfigInfo(
            GameProviderId: gameProviderId,
            GameProviderCode: GameProviderType.AVIAGaming,
            GameProviderConfigId: gameProviderConfigurationId,
            Name: $"{GameProviderType.AVIAGaming}-GPI",
            Description: $"{GameProviderType.AVIAGaming}-GPI Configuration",
            ClientId: $"{GameProviderType.AVIAGaming}-GPI",
            SecretKey: Guid.NewGuid().ToString("N"),
            Status: GameProviderConfigStatus.Active,
            DesktopEndpoint: string.Empty,
            MobileEndpoint: string.Empty,
            ConfigurationData: new JsonObject { }.ToJsonString(),
            IsDefault: false));

        var repository = new GameProviderRepository(fakeClient);

        var actual = await repository.GetAVIAGamingConfigurationAsync(gameProviderConfigurationId);

        Assert.NotNull(actual);
        Assert.Equal(new Uri("http://localhost/"), actual.ApiUrl);
        Assert.Equal("authorization", actual.Authorization);
        Assert.Equal("TgsP@ssw0rd", actual.PlayerPassword);
    }

    [Fact]
    public async Task Get_AwcAelotto_Configuration()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();

        var gameProviderId = Guid.NewGuid();
        var gameProviderConfigurationId = Guid.NewGuid();

        var settingConfiguration = new AwcAelottoConfiguration()
        {
            ApiUrl = new("http://localhost/"),
            ApiRecordUrl = new("http://localhost:8090/"),
        };

        _ = fakeClient
            .SettingGetConfigurationAsync<AwcAelottoConfiguration>(Arg.Is(GameProviderConfigurations.AWC_AELOTTO))
            .Returns(settingConfiguration);

        _ = fakeClient.GameProviderConfigurationGetAsync(gameProviderConfigurationId).Returns(new GameProviderConfigInfo(
            GameProviderId: gameProviderId,
            GameProviderCode: GameProviderType.AWC_AELOTTO,
            GameProviderConfigId: gameProviderConfigurationId,
            Name: $"{GameProviderType.AWC_AELOTTO}-GPI",
            Description: $"{GameProviderType.AWC_AELOTTO}-GPI Configuration",
            ClientId: $"{GameProviderType.AWC_AELOTTO}-GPI",
            SecretKey: Guid.NewGuid().ToString("N"),
            Status: GameProviderConfigStatus.Active,
            DesktopEndpoint: string.Empty,
            MobileEndpoint: string.Empty,
            ConfigurationData: new JsonObject
            {
                ["agentId"] = "agentId",
                ["cert"] = "cert",
            }.ToJsonString(),
            IsDefault: false));

        var repository = new GameProviderRepository(fakeClient);

        var actual = await repository.GetAwcAelottoConfigurationAsync(gameProviderConfigurationId);

        Assert.NotNull(actual);
        Assert.Equal(new Uri("http://localhost/"), actual.ApiUrl);
        Assert.Equal(new Uri("http://localhost:8090/"), actual.ApiRecordUrl);
        Assert.Equal("agentId", actual.AgentId);
        Assert.Equal("cert", actual.Cert);
    }

    [Fact]
    public async Task Get_AwcAws_Configuration()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();

        var gameProviderId = Guid.NewGuid();
        var gameProviderConfigurationId = Guid.NewGuid();

        var settingConfiguration = new AwcAwsConfiguration()
        {
            ApiUrl = new("http://localhost/"),
            ApiRecordUrl = new("http://localhost:8090/"),
        };

        _ = fakeClient
            .SettingGetConfigurationAsync<AwcAwsConfiguration>(Arg.Is(GameProviderConfigurations.AWC_AWS))
            .Returns(settingConfiguration);

        _ = fakeClient.GameProviderConfigurationGetAsync(gameProviderConfigurationId).Returns(new GameProviderConfigInfo(
            GameProviderId: gameProviderId,
            GameProviderCode: GameProviderType.AWC_AWS,
            GameProviderConfigId: gameProviderConfigurationId,
            Name: $"{GameProviderType.AWC_AWS}-GPI",
            Description: $"{GameProviderType.AWC_AWS}-GPI Configuration",
            ClientId: $"{GameProviderType.AWC_AWS}-GPI",
            SecretKey: Guid.NewGuid().ToString("N"),
            Status: GameProviderConfigStatus.Active,
            DesktopEndpoint: string.Empty,
            MobileEndpoint: string.Empty,
            ConfigurationData: new JsonObject
            {
                ["agentId"] = "agentId",
                ["cert"] = "cert",
            }.ToJsonString(),
            IsDefault: false));

        var repository = new GameProviderRepository(fakeClient);

        var actual = await repository.GetAwcAwsConfigurationAsync(gameProviderConfigurationId);

        Assert.NotNull(actual);
        Assert.Equal(new Uri("http://localhost/"), actual.ApiUrl);
        Assert.Equal(new Uri("http://localhost:8090/"), actual.ApiRecordUrl);
        Assert.Equal("agentId", actual.AgentId);
        Assert.Equal("cert", actual.Cert);
    }

    [Fact]
    public async Task Get_AwcCockfight_Configuration()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();

        var gameProviderId = Guid.NewGuid();
        var gameProviderConfigurationId = Guid.NewGuid();

        var settingConfiguration = new AwcCockfightConfiguration()
        {
            ApiUrl = new("http://localhost/"),
            ApiRecordUrl = new("http://localhost:8090/"),
        };

        _ = fakeClient
            .SettingGetConfigurationAsync<AwcCockfightConfiguration>(Arg.Is(GameProviderConfigurations.AWC_COCKFIGHT))
            .Returns(settingConfiguration);

        var betLimit = new JsonObject
        {
            ["SV388"] = new JsonObject
            {
                ["LIVE"] = new JsonObject
                {
                    ["maxbet"] = 2400,
                    ["minbet"] = 1,
                    ["mindraw"] = 1,
                    ["matchlimit"] = 4800,
                    ["maxdraw"] = 1200
                }
            }
        };
        _ = fakeClient.GameProviderConfigurationGetAsync(gameProviderConfigurationId).Returns(new GameProviderConfigInfo(
            GameProviderId: gameProviderId,
            GameProviderCode: GameProviderType.AWC_COCKFIGHT,
            GameProviderConfigId: gameProviderConfigurationId,
            Name: $"{GameProviderType.AWC_COCKFIGHT}-GPI",
            Description: $"{GameProviderType.AWC_COCKFIGHT}-GPI Configuration",
            ClientId: $"{GameProviderType.AWC_COCKFIGHT}-GPI",
            SecretKey: Guid.NewGuid().ToString("N"),
            Status: GameProviderConfigStatus.Active,
            DesktopEndpoint: string.Empty,
            MobileEndpoint: string.Empty,
            ConfigurationData: new JsonObject
            {
                ["agentId"] = "agentId",
                ["cert"] = "cert",
                ["betLimit"] = betLimit
            }.ToJsonString(),
            IsDefault: false));

        var repository = new GameProviderRepository(fakeClient);

        var actual = await repository.GetAwcCockfightConfigurationAsync(gameProviderConfigurationId);

        Assert.NotNull(actual);
        Assert.Equal(new Uri("http://localhost/"), actual.ApiUrl);
        Assert.Equal(new Uri("http://localhost:8090/"), actual.ApiRecordUrl);
        Assert.Equal("agentId", actual.AgentId);
        Assert.Equal("cert", actual.Cert);
        Assert.Equal(betLimit.ToJsonString(), actual.BetLimit.ToJsonString());
    }

    [Fact]
    public async Task Get_AwcFacai_Configuration()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();

        var gameProviderId = Guid.NewGuid();
        var gameProviderConfigurationId = Guid.NewGuid();

        var settingConfiguration = new AwcFacaiConfiguration()
        {
            ApiUrl = new("http://localhost/"),
            ApiRecordUrl = new("http://localhost:8090/"),
        };

        _ = fakeClient
            .SettingGetConfigurationAsync<AwcFacaiConfiguration>(Arg.Is(GameProviderConfigurations.AWC_FACAI))
            .Returns(settingConfiguration);

        _ = fakeClient.GameProviderConfigurationGetAsync(gameProviderConfigurationId).Returns(new GameProviderConfigInfo(
            GameProviderId: gameProviderId,
            GameProviderCode: GameProviderType.AWC_FACAI,
            GameProviderConfigId: gameProviderConfigurationId,
            Name: $"{GameProviderType.AWC_FACAI}-GPI",
            Description: $"{GameProviderType.AWC_FACAI}-GPI Configuration",
            ClientId: $"{GameProviderType.AWC_FACAI}-GPI",
            SecretKey: Guid.NewGuid().ToString("N"),
            Status: GameProviderConfigStatus.Active,
            DesktopEndpoint: string.Empty,
            MobileEndpoint: string.Empty,
            ConfigurationData: new JsonObject
            {
                ["agentId"] = "agentId",
                ["cert"] = "cert"
            }.ToJsonString(),
            IsDefault: false));

        var repository = new GameProviderRepository(fakeClient);

        var actual = await repository.GetAwcFacaiConfigurationAsync(gameProviderConfigurationId);

        Assert.NotNull(actual);
        Assert.Equal(new Uri("http://localhost/"), actual.ApiUrl);
        Assert.Equal(new Uri("http://localhost:8090/"), actual.ApiRecordUrl);
        Assert.Equal("agentId", actual.AgentId);
        Assert.Equal("cert", actual.Cert);
    }

    [Fact]
    public async Task Get_AwcFs_ConfigurationAsync()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();

        var gameProviderId = Guid.NewGuid();
        var gameProviderConfigurationId = Guid.NewGuid();

        var settingConfiguration = new AwcFsConfiguration()
        {
            ApiUrl = new("http://localhost/"),
            ApiRecordUrl = new("http://localhost:8090/"),
        };

        _ = fakeClient
            .SettingGetConfigurationAsync<AwcFsConfiguration>(Arg.Is(GameProviderConfigurations.AWC_FS))
            .Returns(settingConfiguration);

        _ = fakeClient.GameProviderConfigurationGetAsync(gameProviderConfigurationId).Returns(new GameProviderConfigInfo(
            GameProviderId: gameProviderId,
            GameProviderCode: GameProviderType.AWC_FS,
            GameProviderConfigId: gameProviderConfigurationId,
            Name: $"{GameProviderType.AWC_FS}-GPI",
            Description: $"{GameProviderType.AWC_FS}-GPI Configuration",
            ClientId: $"{GameProviderType.AWC_FS}-GPI",
            SecretKey: Guid.NewGuid().ToString("N"),
            Status: GameProviderConfigStatus.Active,
            DesktopEndpoint: string.Empty,
            MobileEndpoint: string.Empty,
            ConfigurationData: new JsonObject
            {
                ["agentId"] = "agentId",
                ["cert"] = "cert"
            }.ToJsonString(),
            IsDefault: false));

        var repository = new GameProviderRepository(fakeClient);

        var actual = await repository.GetAwcFsConfigurationAsync(gameProviderConfigurationId);

        Assert.NotNull(actual);
        Assert.Equal(new Uri("http://localhost/"), actual.ApiUrl);
        Assert.Equal(new Uri("http://localhost:8090/"), actual.ApiRecordUrl);
        Assert.Equal("agentId", actual.AgentId);
        Assert.Equal("cert", actual.Cert);
    }

    [Fact]
    public async Task Get_AwcKm_ConfigurationAsync()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();

        var gameProviderId = Guid.NewGuid();
        var gameProviderConfigurationId = Guid.NewGuid();

        var settingConfiguration = new AwcKmConfiguration()
        {
            ApiUrl = new("http://localhost/"),
            ApiRecordUrl = new("http://localhost:8090/"),
        };

        _ = fakeClient
            .SettingGetConfigurationAsync<AwcKmConfiguration>(Arg.Is(GameProviderConfigurations.AWC_KM))
            .Returns(settingConfiguration);

        _ = fakeClient.GameProviderConfigurationGetAsync(gameProviderConfigurationId).Returns(new GameProviderConfigInfo(
            GameProviderId: gameProviderId,
            GameProviderCode: GameProviderType.AWC_KM,
            GameProviderConfigId: gameProviderConfigurationId,
            Name: $"{GameProviderType.AWC_KM}-GPI",
            Description: $"{GameProviderType.AWC_KM}-GPI Configuration",
            ClientId: $"{GameProviderType.AWC_KM}-GPI-GPI",
            SecretKey: Guid.NewGuid().ToString("N"),
            Status: GameProviderConfigStatus.Active,
            DesktopEndpoint: string.Empty,
            MobileEndpoint: string.Empty,
            ConfigurationData: new JsonObject
            {
                ["agentId"] = "agentId",
                ["cert"] = "cert"
            }.ToJsonString(),
            IsDefault: false));

        var repository = new GameProviderRepository(fakeClient);

        var actual = await repository.GetAwcKmConfigurationAsync(gameProviderConfigurationId);

        Assert.NotNull(actual);
        Assert.Equal(new Uri("http://localhost/"), actual.ApiUrl);
        Assert.Equal(new Uri("http://localhost:8090/"), actual.ApiRecordUrl);
        Assert.Equal("agentId", actual.AgentId);
        Assert.Equal("cert", actual.Cert);
    }

    [Fact]
    public async Task Get_AwcLudo_ConfigurationAsync()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();

        var gameProviderId = Guid.NewGuid();
        var gameProviderConfigurationId = Guid.NewGuid();

        var settingConfiguration = new AwcLudoConfiguration()
        {
            ApiUrl = new("http://localhost/"),
            ApiRecordUrl = new("http://localhost:8090/"),
        };

        _ = fakeClient
            .SettingGetConfigurationAsync<AwcLudoConfiguration>(Arg.Is(GameProviderConfigurations.AWC_LUDO))
            .Returns(settingConfiguration);

        _ = fakeClient.GameProviderConfigurationGetAsync(gameProviderConfigurationId).Returns(new GameProviderConfigInfo(
            GameProviderId: gameProviderId,
            GameProviderCode: GameProviderType.AWC_LUDO,
            GameProviderConfigId: gameProviderConfigurationId,
            Name: $"{GameProviderType.AWC_LUDO}-GPI",
            Description: $"{GameProviderType.AWC_LUDO}-GPI Configuration",
            ClientId: $"{GameProviderType.AWC_LUDO}-GPI",
            SecretKey: Guid.NewGuid().ToString("N"),
            Status: GameProviderConfigStatus.Active,
            DesktopEndpoint: string.Empty,
            MobileEndpoint: string.Empty,
            ConfigurationData: new JsonObject
            {
                ["agentId"] = "agentId",
                ["cert"] = "cert"
            }.ToJsonString(),
            IsDefault: false));

        var repository = new GameProviderRepository(fakeClient);

        var actual = await repository.GetAwcLudoConfigurationAsync(gameProviderConfigurationId);

        Assert.NotNull(actual);
        Assert.Equal(new Uri("http://localhost/"), actual.ApiUrl);
        Assert.Equal(new Uri("http://localhost:8090/"), actual.ApiRecordUrl);
        Assert.Equal("agentId", actual.AgentId);
        Assert.Equal("cert", actual.Cert);
    }

    [Fact]
    public async Task Get_AwcPgs_ConfigurationAsync()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();

        var gameProviderId = Guid.NewGuid();
        var gameProviderConfigurationId = Guid.NewGuid();

        var settingConfiguration = new AwcPgsConfiguration()
        {
            ApiUrl = new("http://localhost/"),
            ApiRecordUrl = new("http://localhost:8090/"),
        };

        _ = fakeClient
            .SettingGetConfigurationAsync<AwcPgsConfiguration>(Arg.Is(GameProviderConfigurations.AWC_PGS))
            .Returns(settingConfiguration);

        _ = fakeClient.GameProviderConfigurationGetAsync(gameProviderConfigurationId).Returns(new GameProviderConfigInfo(
            GameProviderId: gameProviderId,
            GameProviderCode: GameProviderType.AWC_PGS,
            GameProviderConfigId: gameProviderConfigurationId,
            Name: $"{GameProviderType.AWC_PGS}-GPI",
            Description: $"{GameProviderType.AWC_PGS}-GPI Configuration",
            ClientId: $"{GameProviderType.AWC_PGS}-GPI",
            SecretKey: Guid.NewGuid().ToString("N"),
            Status: GameProviderConfigStatus.Active,
            DesktopEndpoint: string.Empty,
            MobileEndpoint: string.Empty,
            ConfigurationData: new JsonObject
            {
                ["agentId"] = "agentId",
                ["cert"] = "cert"
            }.ToJsonString(),
            IsDefault: false));

        var repository = new GameProviderRepository(fakeClient);

        var actual = await repository.GetAwcPgsConfigurationAsync(gameProviderConfigurationId);

        Assert.NotNull(actual);
        Assert.Equal(new Uri("http://localhost/"), actual.ApiUrl);
        Assert.Equal(new Uri("http://localhost:8090/"), actual.ApiRecordUrl);
        Assert.Equal("agentId", actual.AgentId);
        Assert.Equal("cert", actual.Cert);
    }

    [Fact]
    public async Task Get_AwcRcb_ConfigurationAsync()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();

        var gameProviderId = Guid.NewGuid();
        var gameProviderConfigurationId = Guid.NewGuid();

        var settingConfiguration = new AwcRcbConfiguration()
        {
            ApiUrl = new("http://localhost/"),
            ApiRecordUrl = new("http://localhost:8090/"),
        };

        _ = fakeClient
            .SettingGetConfigurationAsync<AwcRcbConfiguration>(Arg.Is(GameProviderConfigurations.AWC_RCB))
            .Returns(settingConfiguration);

        var betLimit = new JsonObject
        {
            ["HORSEBOOK"] = new JsonObject
            {
                ["LIVE"] = new JsonObject
                {
                    ["minbet"] = 10,
                    ["maxbet"] = 3000,
                    ["maxBetSumPerHorse"] = 10000,
                    ["minorMinbet"] = 10,
                    ["minorMaxbet"] = 1000,
                    ["minorMaxBetSumPerHorse"] = 10000
                }
            }
        };
        _ = fakeClient.GameProviderConfigurationGetAsync(gameProviderConfigurationId).Returns(new GameProviderConfigInfo(
            GameProviderId: gameProviderId,
            GameProviderCode: GameProviderType.AWC_RCB,
            GameProviderConfigId: gameProviderConfigurationId,
            Name: $"{GameProviderType.AWC_RCB}-GPI",
            Description: $"{GameProviderType.AWC_RCB}-GPI Configuration",
            ClientId: $"{GameProviderType.AWC_RCB}-GPI",
            SecretKey: Guid.NewGuid().ToString("N"),
            Status: GameProviderConfigStatus.Active,
            DesktopEndpoint: string.Empty,
            MobileEndpoint: string.Empty,
            ConfigurationData: new JsonObject
            {
                ["agentId"] = "agentId",
                ["cert"] = "cert",
                ["betLimit"] = betLimit
            }.ToJsonString(),
            IsDefault: false));

        var repository = new GameProviderRepository(fakeClient);

        var actual = await repository.GetAwcRcbConfigurationAsync(gameProviderConfigurationId);

        Assert.NotNull(actual);
        Assert.Equal(new Uri("http://localhost/"), actual.ApiUrl);
        Assert.Equal(new Uri("http://localhost:8090/"), actual.ApiRecordUrl);
        Assert.Equal("agentId", actual.AgentId);
        Assert.Equal("cert", actual.Cert);
        Assert.Equal(betLimit.ToJsonString(), actual.BetLimit.ToJsonString());
    }

    [Fact]
    public async Task Get_AwcSg_ConfigurationAsync()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();

        var gameProviderId = Guid.NewGuid();
        var gameProviderConfigurationId = Guid.NewGuid();

        var settingConfiguration = new AwcSgConfiguration()
        {
            ApiUrl = new("http://localhost/"),
            ApiRecordUrl = new("http://localhost:8090/"),
        };

        _ = fakeClient
            .SettingGetConfigurationAsync<AwcSgConfiguration>(Arg.Is(GameProviderConfigurations.AWC_SG))
            .Returns(settingConfiguration);

        var betLimit = new JsonObject
        {
            ["SEXYBCRT"] = new JsonObject
            {
                ["LIVE"] = new JsonArray
                {
                    280301,
                    280302,
                    280303
                }
            }
        };
        _ = fakeClient.GameProviderConfigurationGetAsync(gameProviderConfigurationId).Returns(new GameProviderConfigInfo(
            GameProviderId: gameProviderId,
            GameProviderCode: GameProviderType.AWC_SG,
            GameProviderConfigId: gameProviderConfigurationId,
            Name: $"{GameProviderType.AWC_SG}-GPI",
            Description: $"{GameProviderType.AWC_SG}-GPI Configuration",
            ClientId: $"{GameProviderType.AWC_SG}-GPI",
            SecretKey: Guid.NewGuid().ToString("N"),
            Status: GameProviderConfigStatus.Active,
            DesktopEndpoint: string.Empty,
            MobileEndpoint: string.Empty,
            ConfigurationData: new JsonObject
            {
                ["agentId"] = "agentId",
                ["cert"] = "cert",
                ["betLimit"] = betLimit
            }.ToJsonString(),
            IsDefault: false));

        var repository = new GameProviderRepository(fakeClient);

        var actual = await repository.GetAwcSgConfigurationAsync(gameProviderConfigurationId);

        Assert.NotNull(actual);
        Assert.Equal(new Uri("http://localhost/"), actual.ApiUrl);
        Assert.Equal(new Uri("http://localhost:8090/"), actual.ApiRecordUrl);
        Assert.Equal("agentId", actual.AgentId);
        Assert.Equal("cert", actual.Cert);
        Assert.Equal(betLimit.ToJsonString(), actual.BetLimit.ToJsonString());
    }

    [Fact]
    public async Task Get_AwcSpade_ConfigurationAsync()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();

        var gameProviderId = Guid.NewGuid();
        var gameProviderConfigurationId = Guid.NewGuid();

        var settingConfiguration = new AwcSpadeConfiguration()
        {
            ApiUrl = new("http://localhost/"),
            ApiRecordUrl = new("http://localhost:8090/"),
        };

        _ = fakeClient
            .SettingGetConfigurationAsync<AwcSpadeConfiguration>(Arg.Is(GameProviderConfigurations.AWC_SPADE))
            .Returns(settingConfiguration);

        _ = fakeClient.GameProviderConfigurationGetAsync(gameProviderConfigurationId).Returns(new GameProviderConfigInfo(
            GameProviderId: gameProviderId,
            GameProviderCode: GameProviderType.AWC_SPADE,
            GameProviderConfigId: gameProviderConfigurationId,
            Name: $"{GameProviderType.AWC_SPADE}-GPI",
            Description: $"{GameProviderType.AWC_SPADE}-GPI Configuration",
            ClientId: $"{GameProviderType.AWC_SPADE}-GPI",
            SecretKey: Guid.NewGuid().ToString("N"),
            Status: GameProviderConfigStatus.Active,
            DesktopEndpoint: string.Empty,
            MobileEndpoint: string.Empty,
            ConfigurationData: new JsonObject
            {
                ["agentId"] = "agentId",
                ["cert"] = "cert"
            }.ToJsonString(),
            IsDefault: false));

        var repository = new GameProviderRepository(fakeClient);

        var actual = await repository.GetAwcSpadeConfigurationAsync(gameProviderConfigurationId);

        Assert.NotNull(actual);
        Assert.Equal(new Uri("http://localhost/"), actual.ApiUrl);
        Assert.Equal(new Uri("http://localhost:8090/"), actual.ApiRecordUrl);
        Assert.Equal("agentId", actual.AgentId);
        Assert.Equal("cert", actual.Cert);
    }

    [Fact]
    public async Task Get_AwcVenus_ConfigurationAsync()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();

        var gameProviderId = Guid.NewGuid();
        var gameProviderConfigurationId = Guid.NewGuid();

        var settingConfiguration = new AwcVenusConfiguration()
        {
            ApiUrl = new("http://localhost/"),
            ApiRecordUrl = new("http://localhost:8090/"),
        };

        _ = fakeClient
            .SettingGetConfigurationAsync<AwcVenusConfiguration>(Arg.Is(GameProviderConfigurations.AWC_VENUS))
            .Returns(settingConfiguration);

        var betLimit = new JsonObject
        {
            ["VENUS"] = new JsonObject
            {
                ["LIVE"] = new JsonObject
                {
                    ["limitId"] = new JsonArray {
                    280303,
                    280304,
                    280305
                    }
                }
            }
        };
        _ = fakeClient.GameProviderConfigurationGetAsync(gameProviderConfigurationId).Returns(new GameProviderConfigInfo(
            GameProviderId: gameProviderId,
            GameProviderCode: GameProviderType.AWC_VENUS,
            GameProviderConfigId: gameProviderConfigurationId,
            Name: $"{GameProviderType.AWC_VENUS}-GPI",
            Description: $"{GameProviderType.AWC_VENUS}-GPI Configuration",
            ClientId: $"{GameProviderType.AWC_VENUS}-GPI",
            SecretKey: Guid.NewGuid().ToString("N"),
            Status: GameProviderConfigStatus.Active,
            DesktopEndpoint: string.Empty,
            MobileEndpoint: string.Empty,
            ConfigurationData: new JsonObject
            {
                ["agentId"] = "agentId",
                ["cert"] = "cert",
                ["betLimit"] = betLimit
            }.ToJsonString(),
            IsDefault: false));

        var repository = new GameProviderRepository(fakeClient);

        var actual = await repository.GetAwcVenusConfigurationAsync(gameProviderConfigurationId);

        Assert.NotNull(actual);
        Assert.Equal(new Uri("http://localhost/"), actual.ApiUrl);
        Assert.Equal(new Uri("http://localhost:8090/"), actual.ApiRecordUrl);
        Assert.Equal("agentId", actual.AgentId);
        Assert.Equal("cert", actual.Cert);
        Assert.Equal(betLimit.ToJsonString(), actual.BetLimit.ToJsonString());
    }

    [Fact]
    public async Task Get_AwcYl_ConfigurationAsync()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();

        var gameProviderId = Guid.NewGuid();
        var gameProviderConfigurationId = Guid.NewGuid();

        var settingConfiguration = new AwcYlConfiguration()
        {
            ApiUrl = new("http://localhost/"),
            ApiRecordUrl = new("http://localhost:8090/"),
        };

        _ = fakeClient
            .SettingGetConfigurationAsync<AwcYlConfiguration>(Arg.Is(GameProviderConfigurations.AWC_YL))
            .Returns(settingConfiguration);

        _ = fakeClient.GameProviderConfigurationGetAsync(gameProviderConfigurationId).Returns(new GameProviderConfigInfo(
            GameProviderId: gameProviderId,
            GameProviderCode: GameProviderType.AWC_YL,
            GameProviderConfigId: gameProviderConfigurationId,
            Name: $"{GameProviderType.AWC_YL}-GPI",
            Description: $"{GameProviderType.AWC_YL}-GPI Configuration",
            ClientId: $"{GameProviderType.AWC_YL}-GPI",
            SecretKey: Guid.NewGuid().ToString("N"),
            Status: GameProviderConfigStatus.Active,
            DesktopEndpoint: string.Empty,
            MobileEndpoint: string.Empty,
            ConfigurationData: new JsonObject
            {
                ["agentId"] = "agentId",
                ["cert"] = "cert"
            }.ToJsonString(),
            IsDefault: false));

        var repository = new GameProviderRepository(fakeClient);

        var actual = await repository.GetAwcYlConfigurationAsync(gameProviderConfigurationId);

        Assert.NotNull(actual);
        Assert.Equal(new Uri("http://localhost/"), actual.ApiUrl);
        Assert.Equal(new Uri("http://localhost:8090/"), actual.ApiRecordUrl);
        Assert.Equal("agentId", actual.AgentId);
        Assert.Equal("cert", actual.Cert);
    }

    [Fact]
    public async Task Get_AwcYesbingo_ConfigurationAsync()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();

        var gameProviderId = Guid.NewGuid();
        var gameProviderConfigurationId = Guid.NewGuid();

        var settingConfiguration = new AwcYesbingoConfiguration()
        {
            ApiUrl = new("http://localhost/"),
            ApiRecordUrl = new("http://localhost:8090/"),
        };

        _ = fakeClient
            .SettingGetConfigurationAsync<AwcYesbingoConfiguration>(Arg.Is(GameProviderConfigurations.AWC_YESBINGO))
            .Returns(settingConfiguration);

        _ = fakeClient.GameProviderConfigurationGetAsync(gameProviderConfigurationId).Returns(new GameProviderConfigInfo(
            GameProviderId: gameProviderId,
            GameProviderCode: GameProviderType.AWC_YESBINGO,
            GameProviderConfigId: gameProviderConfigurationId,
            Name: $"{GameProviderType.AWC_YESBINGO}-GPI",
            Description: $"{GameProviderType.AWC_YESBINGO}-GPI Configuration",
            ClientId: $"{GameProviderType.AWC_YESBINGO}-GPI",
            SecretKey: Guid.NewGuid().ToString("N"),
            Status: GameProviderConfigStatus.Active,
            DesktopEndpoint: string.Empty,
            MobileEndpoint: string.Empty,
            ConfigurationData: new JsonObject
            {
                ["agentId"] = "agentId",
                ["cert"] = "cert"
            }.ToJsonString(),
            IsDefault: false));

        var repository = new GameProviderRepository(fakeClient);

        var actual = await repository.GetAwcYesbingoConfigurationAsync(gameProviderConfigurationId);

        Assert.NotNull(actual);
        Assert.Equal(new Uri("http://localhost/"), actual.ApiUrl);
        Assert.Equal(new Uri("http://localhost:8090/"), actual.ApiRecordUrl);
        Assert.Equal("agentId", actual.AgentId);
        Assert.Equal("cert", actual.Cert);
    }

    [Fact]
    public async Task Get_BTiSports_Configuration()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();

        var gameProviderId = Guid.NewGuid();
        var gameProviderConfigurationId = Guid.NewGuid();

        var settingConfiguration = new BTiSportsConfiguration()
        {
            ApiUrl = new(" https://wallet.bti.bet/"),
            TokenApiUrl = new(" https://asia-east2-bq-data-api.cloudfunctions.net/"),
            HistoryBetsApiUrl = new("https://get-bet-by-id-d5nlg5buca-de.a.run.app/"),
            HistoryBetsPagingApiUrl = new("https://get-history-bets-paging-d5nlg5buca-de.a.run.app/"),
            OpenBetsApiUrl = new("https://get-open-bets-d5nlg5buca-de.a.run.app/"),
            DesktopLayout = "sports",
            MobileLayout = "sports"
        };

        _ = fakeClient
            .SettingGetConfigurationAsync<BTiSportsConfiguration>(Arg.Is(GameProviderConfigurations.BTiSports))
            .Returns(settingConfiguration);

        _ = fakeClient.GameProviderConfigurationGetAsync(gameProviderConfigurationId).Returns(new GameProviderConfigInfo(
            GameProviderId: gameProviderId,
            GameProviderCode: GameProviderType.BTiSports,
            GameProviderConfigId: gameProviderConfigurationId,
            Name: "BTI-GPI",
            Description: "BTI-GPI Configuration",
            ClientId: "BTI-GPI",
            SecretKey: Guid.NewGuid().ToString("N"),
            Status: GameProviderConfigStatus.Active,
            DesktopEndpoint: string.Empty,
            MobileEndpoint: string.Empty,
            ConfigurationData: "{\"SportsbookURL\":\"https://prod20240-108707434.1x2aaa.com\",\"AgentUserName\":\"PegaGaming_GPIRMB\",\"AgentPassword\":\"IM60Ho7Y4B9C\",\"DesktopLayout\":\"asian-view\",\"MobileLayout \":\"sports\"}",
            IsDefault: false));

        var repository = new GameProviderRepository(fakeClient);

        var actual = await repository.GetBTiSportsConfigurationAsync(gameProviderConfigurationId);

        Assert.NotNull(actual);
        Assert.Equal(new Uri("https://prod20240-108707434.1x2aaa.com/"), actual.SportsbookURL);
        Assert.Equal("PegaGaming_GPIRMB", actual.AgentUserName);
        Assert.Equal("IM60Ho7Y4B9C", actual.AgentPassword);
        Assert.Equal("asian-view", actual.DesktopLayout);
        Assert.Equal("sports", actual.MobileLayout);
    }

    [Fact]
    public async Task Get_Cards365Transfer_Configuration()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();

        var gameProviderId = Guid.NewGuid();
        var gameProviderConfigurationId = Guid.NewGuid();

        var settingConfiguration = new Cards365TransferConfiguration()
        {
            ApiUrl = new("https://localhost/"),
            Website = "website",
            Cert = "cert"
        };
        _ = fakeClient
            .SettingGetConfigurationAsync<Cards365TransferConfiguration>(Arg.Is(GameProviderConfigurations.Cards365Transfer))
            .Returns(settingConfiguration);

        _ = fakeClient.GameProviderConfigurationGetAsync(gameProviderConfigurationId).Returns(new GameProviderConfigInfo(
            GameProviderId: gameProviderId,
            GameProviderCode: GameProviderType.Cards365Transfer,
            GameProviderConfigId: gameProviderConfigurationId,
            Name: "365TF-GPI",
            Description: "365TF-GPI Configuration",
            ClientId: "365TF-GPI",
            SecretKey: Guid.NewGuid().ToString("N"),
            Status: GameProviderConfigStatus.Active,
            DesktopEndpoint: string.Empty,
            MobileEndpoint: string.Empty,
            ConfigurationData: "{\"Extension1\":\"cardp101\"}",
            IsDefault: false));

        var repository = new GameProviderRepository(fakeClient);

        var actual = await repository.GetCards365TransferConfigurationAsync(gameProviderConfigurationId);

        Assert.NotNull(actual);
        Assert.Equal(new("https://localhost/"), actual.ApiUrl);
        Assert.Equal("website", actual.Website);
        Assert.Equal("cert", actual.Cert);
        Assert.Equal("cardp101", actual.Extension1);
    }

    [Fact]
    public async Task Get_CrPati_Configuration()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();

        var gameProviderId = Guid.NewGuid();
        var gameProviderConfigurationId = Guid.NewGuid();

        var settingConfiguration = new CrPatiConfiguration()
        {
            ApiUrl = new("https://localhost/")
        };
        _ = fakeClient
            .SettingGetConfigurationAsync<CrPatiConfiguration>(Arg.Is(GameProviderConfigurations.CrPati))
            .Returns(settingConfiguration);

        _ = fakeClient.GameProviderConfigurationGetAsync(gameProviderConfigurationId).Returns(new GameProviderConfigInfo(
            GameProviderId: gameProviderId,
            GameProviderCode: GameProviderType.CrPati,
            GameProviderConfigId: gameProviderConfigurationId,
            Name: "CP-GPI",
            Description: "CP-GPI Configuration",
            ClientId: "CP-GPI",
            SecretKey: Guid.NewGuid().ToString("N"),
            Status: GameProviderConfigStatus.Active,
            DesktopEndpoint: string.Empty,
            MobileEndpoint: string.Empty,
            ConfigurationData: new JsonObject
            {
                ["ApiKey"] = "testgpi123123",
                ["ApiSecret"] = "qOV55DQYYItjW1qPArpdLNQpWY3ykFoJi4TRF86CA1A9O4pQwR",
            }.ToJsonString(),
            IsDefault: false));

        var repository = new GameProviderRepository(fakeClient);

        var actual = await repository.GetCrPatiConfigurationAsync(gameProviderConfigurationId);

        Assert.NotNull(actual);
        Assert.Equal(new("https://localhost/"), actual.ApiUrl);
        Assert.Equal("testgpi123123", actual.ApiKey);
        Assert.Equal("qOV55DQYYItjW1qPArpdLNQpWY3ykFoJi4TRF86CA1A9O4pQwR", actual.ApiSecret);
    }

    [Fact]
    public async Task Get_Game_Endpoints()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();

        var desktopEndpoint = "http://desktop.google.com";
        var mobileEndpoint = "http://mobile.google.com";
        var gameProviderId = Guid.NewGuid();
        var gameProviderConfigurationId = Guid.NewGuid();

        _ = fakeClient.GameProviderConfigurationGetAsync(gameProviderConfigurationId).Returns(new GameProviderConfigInfo(
            GameProviderId: gameProviderId,
            GameProviderCode: "AG",
            GameProviderConfigId: gameProviderConfigurationId,
            Name: "AG-PG",
            Description: "AG-PG Configuration",
            ClientId: "AG-PG",
            SecretKey: Guid.NewGuid().ToString("N"),
            Status: GameProviderConfigStatus.Active,
            DesktopEndpoint: desktopEndpoint,
            MobileEndpoint: mobileEndpoint,
            ConfigurationData: string.Empty,
            IsDefault: false));

        var repository = new GameProviderRepository(fakeClient);

        var (DesktopEndpoint, MobileEndpoint) = await repository.GetGameEndpointsAsync(gameProviderConfigurationId);

        Assert.Equal(desktopEndpoint, DesktopEndpoint);
        Assert.Equal(mobileEndpoint, MobileEndpoint);
    }

    [Fact]
    public async Task Get_GameProvider()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();

        var gameProviderId = Guid.NewGuid();

        _ = fakeClient.GameProviderGetAsync(gameProviderId).Returns(new GameProviderInfo(
            GameProviderId: gameProviderId,
            Description: "KY",
            AutoCloseRound: true,
            SupportTestPlayer: true,
            AllowKickPlayer: true,
            PlaceBetTimeout: 0,
            SettleBetTimeout: 0,
            UniqueId: 1,
            TransferWallet: true,
            GameProviderCode: "KY",
            Name: "KY",
            Localization: null));

        var repository = new GameProviderRepository(fakeClient);

        var actual = await repository.GetAsync(gameProviderId);

        Assert.NotNull(actual);
        Assert.Equal(gameProviderId, actual.GameProviderId);
    }

    [Fact]
    public async Task Get_GameProvider_CultureCode()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();

        var gameProviderId = Guid.NewGuid();
        var cultureCode = "en-US";
        var expected = "en";
        _ = fakeClient.GameProviderGetCultureCodeAsync(gameProviderId, cultureCode).Returns(expected);

        var repository = new GameProviderRepository(fakeClient);

        var actual = await repository.GetGameProviderCultureCodeAsync(gameProviderId, cultureCode);

        Assert.NotNull(actual);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task Get_GameProvider_CurrencyCode()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();

        var gameProviderId = Guid.NewGuid();
        var currencyCode = "CNY";
        var expected = "RMB";
        _ = fakeClient.GameProviderGetCurrencyCodeAsync(gameProviderId, currencyCode).Returns(expected);

        var repository = new GameProviderRepository(fakeClient);

        var actual = await repository.GetGameProviderCurrencyCodeAsync(gameProviderId, currencyCode);

        Assert.NotNull(actual);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task Get_GameProvider_Id()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();

        var gameProviderCode = "KY";
        var expected = Guid.NewGuid();
        _ = fakeClient.GameProviderGetGameProviderIdAsync(gameProviderCode).Returns(expected);

        var repository = new GameProviderRepository(fakeClient);

        var actual = await repository.GetGameProviderIdAsync(gameProviderCode);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task Get_GKXGaming_Configuration()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();

        var gameProviderId = Guid.NewGuid();
        var gameProviderConfigurationId = Guid.NewGuid();

        var settingConfiguration = new GKXGamingConfiguration()
        {
            AmountFormat = "10,2",
            AmountMaximum = 10000000m,
            ApiUrl = new("https://wc-api.orz16.com/"),
            ApiRecordUrl = new("https://wc-record.orz16.com/")
        };

        _ = fakeClient
            .SettingGetConfigurationAsync<GKXGamingConfiguration>(Arg.Is(GameProviderConfigurations.GKXGaming))
            .Returns(settingConfiguration);

        _ = fakeClient.GameProviderConfigurationGetAsync(gameProviderConfigurationId).Returns(new GameProviderConfigInfo(
            GameProviderId: gameProviderId,
            GameProviderCode: GameProviderType.GKXGaming,
            GameProviderConfigId: gameProviderConfigurationId,
            Name: $"{GameProviderType.GKXGaming}-GPI",
            Description: $"{GameProviderType.GKXGaming}--GPI Configuration",
            ClientId: $"{GameProviderType.GKXGaming}--GPI",
            SecretKey: Guid.NewGuid().ToString("N"),
            Status: GameProviderConfigStatus.Active,
            DesktopEndpoint: string.Empty,
            MobileEndpoint: string.Empty,
            ConfigurationData: "{\"Deskey\":\"03CFE0179704BCDF\",\"Md5key\":\"337B01054C6FA208\",\"AgentId\":\"60073\"}",
            IsDefault: false));

        var repository = new GameProviderRepository(fakeClient);

        var actual = await repository.GetGKXGamingConfigurationAsync(gameProviderConfigurationId);

        Assert.NotNull(actual);
        Assert.Equal("03CFE0179704BCDF", actual.Deskey);
        Assert.Equal("337B01054C6FA208", actual.Md5key);
        Assert.Equal("60073", actual.AgentId);
    }

    [Fact]
    public async Task Get_InplayMatrixESports_Configuration()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();

        var gameProviderId = Guid.NewGuid();
        var gameProviderConfigurationId = Guid.NewGuid();

        var settingConfiguration = new InplayMatrixESportsConfiguration()
        {
            ApiUrl = new("http://operatorapi.staging.imaegisapi.com/"),
            ReportUrl = new("http://operatorapi-report.staging.imaegisapi.com/"),
            Password = "1q2w3e4r"
        };
        _ = fakeClient
            .SettingGetConfigurationAsync<InplayMatrixESportsConfiguration>(Arg.Is(GameProviderConfigurations.InplayMatrix_ESports))
            .Returns(settingConfiguration);

        _ = fakeClient.GameProviderConfigurationGetAsync(gameProviderConfigurationId).Returns(new GameProviderConfigInfo(
            GameProviderId: gameProviderId,
            GameProviderCode: GameProviderType.InplayMatrix_ESports,
            GameProviderConfigId: gameProviderConfigurationId,
            Name: "InplayMatrix_ESports-GPI",
            Description: "InplayMatrix_ESports-GPI Configuration",
            ClientId: "InplayMatrix_ESports-GPI",
            SecretKey: Guid.NewGuid().ToString("N"),
            Status: GameProviderConfigStatus.Active,
            DesktopEndpoint: string.Empty,
            MobileEndpoint: string.Empty,
            ConfigurationData: "{\"MerchantCode\":\"OfL5H2hKOIP9tHp9ZIoqYeggJtFC39zQ\",\"MerchantPrefix\":\"IM1S3\"}",
            IsDefault: false));

        var repository = new GameProviderRepository(fakeClient);

        var actual = await repository.GetInplayMatrixESportsConfigurationAsync(gameProviderConfigurationId);

        Assert.NotNull(actual);
        Assert.Equal("OfL5H2hKOIP9tHp9ZIoqYeggJtFC39zQ", actual.MerchantCode);
        Assert.Equal("IM1S3", actual.MerchantPrefix);
    }

    [Fact]
    public async Task Get_JDBTransfer_Configuration()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();

        var gameProviderId = Guid.NewGuid();
        var gameProviderConfigurationId = Guid.NewGuid();

        var settingConfiguration = new JDBTransferConfiguration()
        {
            ApiUrl = new("http://api.jdb247.net/"),
            Domain = "ZF",
            AesIV = "SettingConfiguration-AesIV",
            AesKey = "SettingConfiguration-AesKey"
        };
        _ = fakeClient
            .SettingGetConfigurationAsync<JDBTransferConfiguration>(Arg.Is(GameProviderConfigurations.JDBTransfer))
            .Returns(settingConfiguration);

        _ = fakeClient.GameProviderConfigurationGetAsync(gameProviderConfigurationId).Returns(new GameProviderConfigInfo(
            GameProviderId: gameProviderId,
            GameProviderCode: GameProviderType.JDBTransfer,
            GameProviderConfigId: gameProviderConfigurationId,
            Name: "JDBTF-GPI",
            Description: "JDBTF-GPI Configuration",
            ClientId: "JDBTF-GPI",
            SecretKey: Guid.NewGuid().ToString("N"),
            Status: GameProviderConfigStatus.Active,
            DesktopEndpoint: string.Empty,
            MobileEndpoint: string.Empty,
            ConfigurationData: "{\"Agent\":\"ConfigurationData-Agent\",\"AesIV\":\"ConfigurationData-AesIV\",\"AesKey\":\"ConfigurationData-AesKey\"}",
            IsDefault: false));

        var repository = new GameProviderRepository(fakeClient);

        var actual = await repository.GetJDBTransferConfigurationAsync(gameProviderConfigurationId);

        Assert.NotNull(actual);
        Assert.Equal("ConfigurationData-Agent", actual.Agent);
        Assert.Equal("ConfigurationData-AesIV", actual.AesIV);
        Assert.Equal("ConfigurationData-AesKey", actual.AesKey);
    }

    [Fact]
    public async Task Get_JiliTransfer_Configuration()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();

        var gameProviderId = Guid.NewGuid();
        var gameProviderConfigurationId = Guid.NewGuid();

        var settingConfiguration = new JiliTransferConfiguration()
        {
            ApiUrl = new("https://wb-api.jlfafafa2.com/"),
            AgentId = "SettingConfigurationAgentId",
            AgentKey = "df65d2b1d7d42638c4c14a948c6d12bd348334b4",
        };
        _ = fakeClient
            .SettingGetConfigurationAsync<JiliTransferConfiguration>(Arg.Is(GameProviderConfigurations.JiliTransfer))
            .Returns(settingConfiguration);

        _ = fakeClient.GameProviderConfigurationGetAsync(gameProviderConfigurationId).Returns(new GameProviderConfigInfo(
            GameProviderId: gameProviderId,
            GameProviderCode: GameProviderType.JiliTransfer,
            GameProviderConfigId: gameProviderConfigurationId,
            Name: "JILITF-GPI",
            Description: "JILITF-GPI Configuration",
            ClientId: "JILITF-GPI",
            SecretKey: Guid.NewGuid().ToString("N"),
            Status: GameProviderConfigStatus.Active,
            DesktopEndpoint: string.Empty,
            MobileEndpoint: string.Empty,
            ConfigurationData: "{\"AgentId\":\"ConfigurationData-AgentId\"}",
            IsDefault: false));

        var repository = new GameProviderRepository(fakeClient);

        var actual = await repository.GetJiliTransferConfigurationAsync(gameProviderConfigurationId);

        Assert.NotNull(actual);
        Assert.Equal("ConfigurationData-AgentId", actual.AgentId);
    }

    [Fact]
    public async Task Get_Joker_Configuration()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();

        var gameProviderId = Guid.NewGuid();
        var gameProviderConfigurationId = Guid.NewGuid();

        var settingConfiguration = new JokerConfiguration()
        {
            ApiUrl = new("http://localhost/"),
            GameLaunchUrl = new("http://localhost/")
        };
        _ = fakeClient
            .SettingGetConfigurationAsync<JokerConfiguration>(Arg.Is(GameProviderConfigurations.Joker))
            .Returns(settingConfiguration);

        _ = fakeClient.GameProviderConfigurationGetAsync(gameProviderConfigurationId).Returns(new GameProviderConfigInfo(
            GameProviderId: gameProviderId,
            GameProviderCode: GameProviderType.WMCasinoTransfer,
            GameProviderConfigId: gameProviderConfigurationId,
            Name: $"{GameProviderType.Joker}-GPI",
            Description: $"{GameProviderType.Joker}-GPI Configuration",
            ClientId: $"{GameProviderType.Joker}-GPI",
            SecretKey: Guid.NewGuid().ToString("N"),
            Status: GameProviderConfigStatus.Active,
            DesktopEndpoint: string.Empty,
            MobileEndpoint: string.Empty,
            ConfigurationData: "{\"AppId\":\"TGVY\",\"SecretKey\":\"s5ghrn77baoto\"}",
            IsDefault: false));

        var repository = new GameProviderRepository(fakeClient);

        var actual = await repository.GetJokerConfigurationAsync(gameProviderConfigurationId);

        Assert.NotNull(actual);
        Assert.Equal("TGVY", actual.AppId);
        Assert.Equal("s5ghrn77baoto", actual.SecretKey);
    }

    [Fact]
    public async Task Get_LEGaming_ConfigurationData()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();

        var gameProviderId = Guid.NewGuid();
        var gameProviderConfigurationId = Guid.NewGuid();

        var settingConfiguration = new LEGamingConfiguration()
        {
            AmountMaximum = 999999999999,
            AmountFormat = "12,2",
            ApiUrl = new("http://localhost/"),
            ApiRecordUrl = new("http://localhost:8090/"),
            Deskey = "deskey",
            Md5key = "md5key"
        };
        _ = fakeClient
            .SettingGetConfigurationAsync<LEGamingConfiguration>(Arg.Is(GameProviderConfigurations.LEGaming))
            .Returns(settingConfiguration);

        _ = fakeClient.GameProviderConfigurationGetAsync(gameProviderConfigurationId).Returns(new GameProviderConfigInfo(
            GameProviderId: gameProviderId,
            GameProviderCode: "LY",
            GameProviderConfigId: gameProviderConfigurationId,
            Name: "LY-GPI",
            Description: "LY-GPI Configuration",
            ClientId: "LY-GPI",
            SecretKey: Guid.NewGuid().ToString("N"),
            Status: GameProviderConfigStatus.Active,
            DesktopEndpoint: string.Empty,
            MobileEndpoint: string.Empty,
            ConfigurationData: "{\"CurrencyCode\":\"HKD\",\"AgentId\":\"200995\"}",
            IsDefault: false));

        var repository = new GameProviderRepository(fakeClient);

        var actual = await repository.GetLEGamingConfigurationAsync(gameProviderConfigurationId);

        Assert.NotNull(actual);
        Assert.Equal(new("http://localhost/"), actual.ApiUrl);
        Assert.Equal(new("http://localhost:8090/"), actual.ApiRecordUrl);
        Assert.Equal("deskey", actual.Deskey);
        Assert.Equal("md5key", actual.Md5key);
        Assert.Equal("HKD", actual.CurrencyCode);
        Assert.Equal("200995", actual.AgentId);
    }

    [Fact]
    public async Task Get_Pinnacle_Configuration()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();

        var gameProviderId = Guid.NewGuid();
        var gameProviderConfigurationId = Guid.NewGuid();

        var settingConfiguration = new PinnacleConfiguration()
        {
            ApiUrl = new("https://paapistg.oreo88.com/"),
            AgentKey = "8d0e9ac0-99d8-48c7-8307-bc328c4181f6",
            SecretKey = "zenDIdY7Knuzmeen"
        };
        _ = fakeClient
            .SettingGetConfigurationAsync<PinnacleConfiguration>(
                key: Arg.Is(GameProviderConfigurations.Pinnacle),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(settingConfiguration);

        _ = fakeClient.GameProviderConfigurationGetAsync(gameProviderConfigurationId).Returns(new GameProviderConfigInfo(
            GameProviderId: gameProviderId,
            GameProviderCode: GameProviderConfigurations.Pinnacle,
            GameProviderConfigId: gameProviderConfigurationId,
            Name: $"{GameProviderConfigurations.Pinnacle}-GPI",
            Description: $"{GameProviderConfigurations.Pinnacle}-GPI Configuration",
            ClientId: $"{GameProviderConfigurations.Pinnacle}-GPI",
            SecretKey: Guid.NewGuid().ToString("N"),
            Status: GameProviderConfigStatus.Active,
            DesktopEndpoint: string.Empty,
            MobileEndpoint: string.Empty,
            ConfigurationData: "{\"AgentCode\":\"V0201\"}",
            IsDefault: false));

        var repository = new GameProviderRepository(fakeClient);

        var actual = await repository.GetPinnacleConfigurationAsync(gameProviderConfigurationId);

        Assert.NotNull(actual);
        Assert.Equal("V0201", actual.AgentCode);
        Assert.Equal("8d0e9ac0-99d8-48c7-8307-bc328c4181f6", actual.AgentKey);
        Assert.Equal("zenDIdY7Knuzmeen", actual.SecretKey);
    }

    [Fact]
    public async Task Get_SabaTransfer_Configuration()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();

        var gameProviderId = Guid.NewGuid();
        var gameProviderConfigurationId = Guid.NewGuid();

        var settingConfiguration = new SabaTransferConfiguration()
        {
            ApiUrl = new("http://h0i2tsa.bw6688.com/"),
            OddsType = 1
        };
        _ = fakeClient
            .SettingGetConfigurationAsync<SabaTransferConfiguration>(Arg.Is(GameProviderConfigurations.SabaTransfer))
            .Returns(settingConfiguration);

        _ = fakeClient.GameProviderConfigurationGetAsync(gameProviderConfigurationId).Returns(new GameProviderConfigInfo(
            GameProviderId: gameProviderId,
            GameProviderCode: GameProviderType.SabaTransfer,
            GameProviderConfigId: gameProviderConfigurationId,
            Name: "SABATF-GPI",
            Description: "SABATF-GPI Configuration",
            ClientId: "SABATF-GPI",
            SecretKey: Guid.NewGuid().ToString("N"),
            Status: GameProviderConfigStatus.Active,
            DesktopEndpoint: string.Empty,
            MobileEndpoint: string.Empty,
            ConfigurationData: "{\"OperatorId\":\"tg\",\"VendorId\":\"um5vspnzkx\",\"OddsType\":2}",
            IsDefault: false));

        var repository = new GameProviderRepository(fakeClient);

        var actual = await repository.GetSabaTransferConfigurationAsync(gameProviderConfigurationId);

        Assert.NotNull(actual);
        Assert.Equal("tg", actual.OperatorId);
        Assert.Equal("um5vspnzkx", actual.VendorId);
        Assert.Equal(2, actual.OddsType);
    }

    [Fact]
    public async Task Get_SBOTransfer_Configuration()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();

        var gameProviderId = Guid.NewGuid();
        var gameProviderConfigurationId = Guid.NewGuid();

        var settingConfiguration = new SBOTransferConfiguration()
        {
            ApiUrl = new("https://ex-api-demo-yy.568win.com/"),
            CompanyKey = "56E0854E61C3468580E491C38405580B",
            Min = 1,
            Max = 5000,
            MaxPerMatch = 20000,
            CasinoTableLimit = 1
        };
        _ = fakeClient
            .SettingGetConfigurationAsync<SBOTransferConfiguration>(Arg.Is(GameProviderConfigurations.SBOTransfer))
            .Returns(settingConfiguration);

        _ = fakeClient.GameProviderConfigurationGetAsync(gameProviderConfigurationId).Returns(new GameProviderConfigInfo(
            GameProviderId: gameProviderId,
            GameProviderCode: "SBOTF",
            GameProviderConfigId: gameProviderConfigurationId,
            Name: "SBOTF-GPI",
            Description: "SBOTF-GPI Configuration",
            ClientId: "SBOTF-GPI",
            SecretKey: Guid.NewGuid().ToString("N"),
            Status: GameProviderConfigStatus.Active,
            DesktopEndpoint: string.Empty,
            MobileEndpoint: string.Empty,
            ConfigurationData: "{\"Agent\":\"STAG_GPI\",\"Currency\":\"CNY\",\"Min\":1,\"Max\":10000,\"MaxPerMatch\":40000,\"CasinoTableLimit\":2}",
            IsDefault: false));

        var repository = new GameProviderRepository(fakeClient);

        var actual = await repository.GetSBOTransferConfigurationAsync(gameProviderConfigurationId);

        Assert.NotNull(actual);
        Assert.Equal("STAG_GPI", actual.Agent);
        Assert.Equal("CNY", actual.Currency);
        Assert.Equal(10000, actual.Max);
        Assert.Equal(40000, actual.MaxPerMatch);
        Assert.Equal(2, actual.CasinoTableLimit);
    }

    [Fact]
    public async Task Get_SM01_Configuration()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();

        var gameProviderId = Guid.NewGuid();
        var gameProviderConfigurationId = Guid.NewGuid();

        var settingConfiguration = new SM01Configuration()
        {
            ApiUrl = new("https://cn.ucfclive.com"),
        };

        _ = fakeClient
            .SettingGetConfigurationAsync<SM01Configuration>(Arg.Is(GameProviderConfigurations.SM01))
            .Returns(settingConfiguration);

        _ = fakeClient.GameProviderConfigurationGetAsync(gameProviderConfigurationId).Returns(new GameProviderConfigInfo(
            GameProviderId: gameProviderId,
            GameProviderCode: GameProviderType.SM01,
            GameProviderConfigId: gameProviderConfigurationId,
            Name: "SM01-GPI",
            Description: "SM01-GPI Configuration",
            ClientId: "SM01-GPI",
            SecretKey: Guid.NewGuid().ToString("N"),
            Status: GameProviderConfigStatus.Active,
            DesktopEndpoint: string.Empty,
            MobileEndpoint: string.Empty,
            ConfigurationData: "{\"MerchantId\":\"SQjCxm2cX5bahBta\",\"MerchantKey\":\"XGKGkcmYNnfex8AecKzCEbD4GMExA2cb\"}",
            IsDefault: false));

        var repository = new GameProviderRepository(fakeClient);

        var actual = await repository.GetSM01ConfigurationAsync(gameProviderConfigurationId);

        Assert.NotNull(actual);
        Assert.Equal(new Uri("https://cn.ucfclive.com"), actual.ApiUrl);
        Assert.Equal("SQjCxm2cX5bahBta", actual.MerchantId);
        Assert.Equal("XGKGkcmYNnfex8AecKzCEbD4GMExA2cb", actual.MerchantKey);
    }

    [Fact]
    public async Task Get_WGFmGaming_ConfigurationData()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();

        var gameProviderId = Guid.NewGuid();
        var gameProviderConfigurationId = Guid.NewGuid();

        var settingConfiguration = new WGFmGamingConfiguration()
        {
            AmountMaximum = 999999999999,
            AmountFormat = "12,2",
            ApiUrl = new("https://api.jsc080.net/"),
            PartnerCode = "SettingConfigurationPartnerCode",
            PartnerKey = "SettingConfigurationPartnerKey",
            IV = "SettingConfigurationPartnerIV",
            Version = 2
        };
        _ = fakeClient
            .SettingGetConfigurationAsync<WGFmGamingConfiguration>(Arg.Is(GameProviderConfigurations.WGFmGaming))
            .Returns(settingConfiguration);

        _ = fakeClient.GameProviderConfigurationGetAsync(gameProviderConfigurationId).Returns(new GameProviderConfigInfo(
            GameProviderId: gameProviderId,
            GameProviderCode: "WGFM",
            GameProviderConfigId: gameProviderConfigurationId,
            Name: "WGFM-GPI",
            Description: "WGFM-GPI Configuration",
            ClientId: "WGFM-GPI",
            SecretKey: Guid.NewGuid().ToString("N"),
            Status: GameProviderConfigStatus.Active,
            DesktopEndpoint: string.Empty,
            MobileEndpoint: string.Empty,
            ConfigurationData: "{\"PartnerCode\":\"ConfigurationDataPartnerCode\",\"PartnerKey\":\"ConfigurationDataPartnerKey\",\"IV\":\"ConfigurationDataPartnerIV\",\"Version\":1}",
            IsDefault: false));

        var repository = new GameProviderRepository(fakeClient);

        var actual = await repository.GetWGFmGamingConfigurationAsync(gameProviderConfigurationId);

        Assert.NotNull(actual);
        Assert.Equal("ConfigurationDataPartnerCode", actual.PartnerCode);
        Assert.Equal("ConfigurationDataPartnerKey", actual.PartnerKey);
        Assert.Equal("ConfigurationDataPartnerIV", actual.IV);
        Assert.Equal(1, actual.Version);
    }

    [Fact]
    public async Task Get_WGFmGaming_ConfigurationData_Is_Empty()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();

        var gameProviderId = Guid.NewGuid();
        var gameProviderConfigurationId = Guid.NewGuid();

        var settingConfiguration = new WGFmGamingConfiguration()
        {
            AmountMaximum = 999999999999,
            AmountFormat = "12,2",
            ApiUrl = new("https://api.jsc080.net/"),
            PartnerCode = "SettingConfigurationPartnerCode",
            PartnerKey = "SettingConfigurationPartnerKey",
            IV = "SettingConfigurationPartnerIV",
            Version = 2
        };
        _ = fakeClient
            .SettingGetConfigurationAsync<WGFmGamingConfiguration>(Arg.Is(GameProviderConfigurations.WGFmGaming))
            .Returns(settingConfiguration);

        _ = fakeClient.GameProviderConfigurationGetAsync(gameProviderConfigurationId).Returns(new GameProviderConfigInfo(
            GameProviderId: gameProviderId,
            GameProviderCode: "WGFM",
            GameProviderConfigId: gameProviderConfigurationId,
            Name: "WGFM-GPI",
            Description: "WGFM-GPI Configuration",
            ClientId: "WGFM-GPI",
            SecretKey: Guid.NewGuid().ToString("N"),
            Status: GameProviderConfigStatus.Active,
            DesktopEndpoint: string.Empty,
            MobileEndpoint: string.Empty,
            ConfigurationData: string.Empty,
            IsDefault: false));

        var repository = new GameProviderRepository(fakeClient);

        var actual = await repository.GetWGFmGamingConfigurationAsync(gameProviderConfigurationId);

        Assert.NotNull(actual);
        Assert.Equal("SettingConfigurationPartnerCode", actual.PartnerCode);
        Assert.Equal("SettingConfigurationPartnerKey", actual.PartnerKey);
        Assert.Equal("SettingConfigurationPartnerIV", actual.IV);
        Assert.Equal(2, actual.Version);
    }

    [Fact]
    public async Task Get_WGViaGaming_ConfigurationData()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();

        var gameProviderId = Guid.NewGuid();
        var gameProviderConfigurationId = Guid.NewGuid();

        var settingConfiguration = new WGViaGamingConfiguration()
        {
            AmountMaximum = 999999999999,
            AmountFormat = "12,2",
            ApiUrl = new("https://localhost/"),
            PartnerCode = "SettingConfigurationPartnerCode",
            PartnerKey = "SettingConfigurationPartnerKey",
            IV = "SettingConfigurationPartnerIV",
        };
        _ = fakeClient
            .SettingGetConfigurationAsync<WGViaGamingConfiguration>(Arg.Is(GameProviderConfigurations.WGViaGaming))
            .Returns(settingConfiguration);

        _ = fakeClient.GameProviderConfigurationGetAsync(gameProviderConfigurationId).Returns(new GameProviderConfigInfo(
            GameProviderId: gameProviderId,
            GameProviderCode: "WGVIA",
            GameProviderConfigId: gameProviderConfigurationId,
            Name: "WGVIA-GPI",
            Description: "WGVIA-GPI Configuration",
            ClientId: "WGVIA-GPI",
            SecretKey: Guid.NewGuid().ToString("N"),
            Status: GameProviderConfigStatus.Active,
            DesktopEndpoint: string.Empty,
            MobileEndpoint: string.Empty,
            ConfigurationData: string.Empty,
            IsDefault: false));

        var repository = new GameProviderRepository(fakeClient);

        var actual = await repository.GetWGViaGamingConfigurationAsync(gameProviderConfigurationId);

        Assert.NotNull(actual);
        Assert.Equal("SettingConfigurationPartnerCode", actual.PartnerCode);
        Assert.Equal("SettingConfigurationPartnerKey", actual.PartnerKey);
        Assert.Equal("SettingConfigurationPartnerIV", actual.IV);
    }

    [Fact]
    public async Task Get_WicketCricket_Configuration()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();

        var gameProviderId = Guid.NewGuid();
        var gameProviderConfigurationId = Guid.NewGuid();

        var settingConfiguration = new WicketCricketConfiguration()
        {
            ApiUrl = new("https://apiinfo.cckk77.net/"),
            ExchangeApiUrl = new("https://test81.cckk77.net/")
        };
        _ = fakeClient
            .SettingGetConfigurationAsync<WicketCricketConfiguration>(Arg.Is(GameProviderConfigurations.WicketCricket))
            .Returns(settingConfiguration);

        _ = fakeClient.GameProviderConfigurationGetAsync(gameProviderConfigurationId).Returns(new GameProviderConfigInfo(
            GameProviderId: gameProviderId,
            GameProviderCode: GameProviderType.WicketCricket,
            GameProviderConfigId: gameProviderConfigurationId,
            Name: "9WCRICKET-GPI_INR",
            Description: "9WCRICKET-GPI_INR Configuration",
            ClientId: "9WCRICKET-GPI_INR",
            SecretKey: Guid.NewGuid().ToString("N"),
            Status: GameProviderConfigStatus.Active,
            DesktopEndpoint: string.Empty,
            MobileEndpoint: string.Empty,
            ConfigurationData: new JsonObject
            {
                ["Website"] = "WICKETSPRO",
                ["Cert"] = "wkVwQ8sMVr252EA9",
                ["Agent"] = "testpginrma"
            }.ToJsonString(),
            IsDefault: false));

        var repository = new GameProviderRepository(fakeClient);

        var actual = await repository.GetWicketCricketConfigurationAsync(gameProviderConfigurationId);

        Assert.NotNull(actual);
        Assert.Equal("WICKETSPRO", actual.Website);
        Assert.Equal("wkVwQ8sMVr252EA9", actual.Cert);
        Assert.Equal("testpginrma", actual.Agent);
    }

    [Fact]
    public async Task Get_WMCasinoTransfer_Configuration()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();

        var gameProviderId = Guid.NewGuid();
        var gameProviderConfigurationId = Guid.NewGuid();

        var settingConfiguration = new WMCasinoTransferConfiguration()
        {
            ApiUrl = new("https://api.a45.me/api/public/Gateway.php")
        };
        _ = fakeClient
            .SettingGetConfigurationAsync<WMCasinoTransferConfiguration>(Arg.Is(GameProviderConfigurations.WMCasinoTransfer))
            .Returns(settingConfiguration);

        _ = fakeClient.GameProviderConfigurationGetAsync(gameProviderConfigurationId).Returns(new GameProviderConfigInfo(
            GameProviderId: gameProviderId,
            GameProviderCode: GameProviderType.WMCasinoTransfer,
            GameProviderConfigId: gameProviderConfigurationId,
            Name: "WMTF-GPI",
            Description: "WMTF-GPI Configuration",
            ClientId: "WMTF-GPI",
            SecretKey: Guid.NewGuid().ToString("N"),
            Status: GameProviderConfigStatus.Active,
            DesktopEndpoint: string.Empty,
            MobileEndpoint: string.Empty,
            ConfigurationData: "{\"vendorId\":\"hpg02twapi\",\"signature\":\"89448b444ed2e922ddc3aa0753370238\"}",
            IsDefault: false));

        var repository = new GameProviderRepository(fakeClient);

        var actual = await repository.GetWMCasinoTransferConfigurationAsync(gameProviderConfigurationId);

        Assert.NotNull(actual);
        Assert.Equal("hpg02twapi", actual.VendorId);
        Assert.Equal("89448b444ed2e922ddc3aa0753370238", actual.Signature);
    }
}
