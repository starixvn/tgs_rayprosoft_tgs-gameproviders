using System.Text.Json;
using ExpectedObjects;
using NSubstitute;
using Tgs.GameCommand.Client;
using Tgs.GameCommand.Common;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.ConfigurationData;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Repositories.UnitTest;

[TestClass]
public class GameProviderRepositoryUnitTest
{
    [TestMethod]
    public async Task Get_Awc_ConfigurationData()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();

        var gameProviderId = Guid.NewGuid();
        var gameProviderConfigurationId = Guid.NewGuid();
        var awcConfigurationData = new AwcConfigurationData(
            AgentId: "testAgentId",
            Cert: "testCert",
            BetLimit: null);
        var configurationData = JsonSerializer.Serialize(awcConfigurationData);

        _ = fakeClient.GameProviderConfigurationGetAsync(gameProviderConfigurationId).Returns(new GameProviderConfigInfo(
            GameProviderId: gameProviderId,
            GameProviderCode: "AWC_AWS",
            GameProviderConfigId: gameProviderConfigurationId,
            Name: "AWC_AWS-GPI",
            Description: "AWC_AWS-GPI Configuration",
            ClientId: "AWC_AWS-GPI",
            SecretKey: Guid.NewGuid().ToString("N"),
            Status: GameProviderConfigStatus.Active,
            DesktopEndpoint: string.Empty,
            MobileEndpoint: string.Empty,
            ConfigurationData: configurationData,
            IsDefault: false));

        var repository = new GameProviderRepository(fakeClient);

        var actual = await repository.GetAwcConfigurationData(gameProviderConfigurationId).ConfigureAwait(false);

        Assert.IsNotNull(actual);
        awcConfigurationData.ToExpectedObject().ShouldEqual(actual);
    }

    [TestMethod]
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

        var actual = await repository.GetBTiSportsConfiguration(gameProviderConfigurationId).ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.AreEqual(new Uri("https://prod20240-108707434.1x2aaa.com/"), actual.SportsbookURL);
        Assert.AreEqual("PegaGaming_GPIRMB", actual.AgentUserName);
        Assert.AreEqual("IM60Ho7Y4B9C", actual.AgentPassword);
        Assert.AreEqual("asian-view", actual.DesktopLayout);
        Assert.AreEqual("sports", actual.MobileLayout);
    }

    [TestMethod]
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

        var actual = await repository.GetGameEndpointsAsync(gameProviderConfigurationId).ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.AreEqual(desktopEndpoint, actual.DesktopEndpoint);
        Assert.AreEqual(mobileEndpoint, actual.MobileEndpoint);
    }

    [TestMethod]
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

        var actual = await repository.GetAsync(gameProviderId).ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.AreEqual(gameProviderId, actual.GameProviderId);
    }

    [TestMethod]
    public async Task Get_GameProvider_CultureCode()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();

        var gameProviderId = Guid.NewGuid();
        var cultureCode = "en-US";
        var expected = "en";
        _ = fakeClient.GameProviderGetCultureCodeAsync(gameProviderId, cultureCode).Returns(expected);

        var repository = new GameProviderRepository(fakeClient);

        var actual = await repository.GetGameProviderCultureCodeAsync(gameProviderId, cultureCode).ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public async Task Get_GameProvider_CurrencyCode()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();

        var gameProviderId = Guid.NewGuid();
        var currencyCode = "CNY";
        var expected = "RMB";
        _ = fakeClient.GameProviderGetCurrencyCodeAsync(gameProviderId, currencyCode).Returns(expected);

        var repository = new GameProviderRepository(fakeClient);

        var actual = await repository.GetGameProviderCurrencyCodeAsync(gameProviderId, currencyCode).ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public async Task Get_GameProvider_Id()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();

        var gameProviderCode = "KY";
        var expected = Guid.NewGuid();
        _ = fakeClient.GameProviderGetGameProviderIdAsync(gameProviderCode).Returns(expected);

        var repository = new GameProviderRepository(fakeClient);

        var actual = await repository.GetGameProviderIdAsync(gameProviderCode).ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
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

        var actual = await repository.GetInplayMatrixESportsConfigurationAsync(gameProviderConfigurationId).ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.AreEqual("OfL5H2hKOIP9tHp9ZIoqYeggJtFC39zQ", actual.MerchantCode);
        Assert.AreEqual("IM1S3", actual.MerchantPrefix);
    }

    [TestMethod]
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

        var actual = await repository.GetJDBTransferConfiguration(gameProviderConfigurationId).ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.AreEqual("ConfigurationData-Agent", actual.Agent);
        Assert.AreEqual("ConfigurationData-AesIV", actual.AesIV);
        Assert.AreEqual("ConfigurationData-AesKey", actual.AesKey);
    }

    [TestMethod]
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

        var actual = await repository.GetJiliTransferConfiguration(gameProviderConfigurationId).ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.AreEqual("ConfigurationData-AgentId", actual.AgentId);
    }

    [TestMethod]
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

        var actual = await repository.GetSabaTransferConfiguration(gameProviderConfigurationId).ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.AreEqual("tg", actual.OperatorId);
        Assert.AreEqual("um5vspnzkx", actual.VendorId);
        Assert.AreEqual(2, actual.OddsType);
    }

    [TestMethod]
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

        var actual = await repository.GetSBOTransferConfiguration(gameProviderConfigurationId).ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.AreEqual("STAG_GPI", actual.Agent);
        Assert.AreEqual("CNY", actual.Currency);
        Assert.AreEqual(10000, actual.Max);
        Assert.AreEqual(40000, actual.MaxPerMatch);
        Assert.AreEqual(2, actual.CasinoTableLimit);
    }

    [TestMethod]
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

        var actual = await repository.GetSM01Configuration(gameProviderConfigurationId).ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.AreEqual(new Uri("https://cn.ucfclive.com"), actual.ApiUrl);
        Assert.AreEqual("SQjCxm2cX5bahBta", actual.MerchantId);
        Assert.AreEqual("XGKGkcmYNnfex8AecKzCEbD4GMExA2cb", actual.MerchantKey);
    }

    [TestMethod]
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
            .SettingGetConfigurationAsync<WGFmGamingConfiguration>(Arg.Is(GameProviderConfigurations.WGSports))
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

        var actual = await repository.GetWGFmGamingConfiguration(gameProviderConfigurationId).ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.AreEqual("ConfigurationDataPartnerCode", actual.PartnerCode);
        Assert.AreEqual("ConfigurationDataPartnerKey", actual.PartnerKey);
        Assert.AreEqual("ConfigurationDataPartnerIV", actual.IV);
        Assert.AreEqual(1, actual.Version);
    }

    [TestMethod]
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
            .SettingGetConfigurationAsync<WGFmGamingConfiguration>(Arg.Is(GameProviderConfigurations.WGSports))
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

        var actual = await repository.GetWGFmGamingConfiguration(gameProviderConfigurationId).ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.AreEqual("SettingConfigurationPartnerCode", actual.PartnerCode);
        Assert.AreEqual("SettingConfigurationPartnerKey", actual.PartnerKey);
        Assert.AreEqual("SettingConfigurationPartnerIV", actual.IV);
        Assert.AreEqual(2, actual.Version);
    }

    [TestMethod]
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

        var actual = await repository.GetWMCasinoTransferConfiguration(gameProviderConfigurationId).ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.AreEqual("hpg02twapi", actual.VendorId);
        Assert.AreEqual("89448b444ed2e922ddc3aa0753370238", actual.Signature);
    }

    [TestMethod]
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

        var actual = await repository.GetCards365TransferConfigurationAsync(gameProviderConfigurationId).ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.AreEqual(new("https://localhost/"), actual.ApiUrl);
        Assert.AreEqual("website", actual.Website);
        Assert.AreEqual("cert", actual.Cert);
        Assert.AreEqual("cardp101", actual.Extension1);
    }
}
