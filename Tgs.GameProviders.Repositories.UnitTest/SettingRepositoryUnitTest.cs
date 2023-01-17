using ExpectedObjects;
using NSubstitute;
using Tgs.GameCommand.Client;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.Domains.SettingConfiguration.KY;

namespace Tgs.GameProviders.Repositories.UnitTest;

[TestClass]
public class SettingRepositoryUnitTest
{
    [TestMethod]
    public async Task Get_Awc_Aelotto_Configuration()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();

        var key = GameProviderConfigurations.AWC_AELOTTO;

        var expected = new AwcAelottoConfiguration()
        {
            ApiUrl = new Uri("http://apiurl.google.com"),
            ApiRecordUrl = new Uri("http://apirecordurl.google.com")
        };
        _ = fakeClient.SettingGetConfigurationAsync<AwcAelottoConfiguration>(Arg.Is(key)).Returns(expected);

        var repository = new SettingRepository(fakeClient);

        var actual = await repository.GetAwcAelottoConfigurationAsync().ConfigureAwait(false);

        Assert.IsNotNull(actual);
        expected.ToExpectedObject().ShouldEqual(actual);
    }

    [TestMethod]
    public async Task Get_Awc_Cockfight_Configuration()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();

        var key = GameProviderConfigurations.AWC_COCKFIGHT;

        var expected = new AwcCockfightConfiguration()
        {
            ApiUrl = new Uri("http://apiurl.google.com"),
            ApiRecordUrl = new Uri("http://apirecordurl.google.com")
        };
        _ = fakeClient.SettingGetConfigurationAsync<AwcCockfightConfiguration>(Arg.Is(key)).Returns(expected);

        var repository = new SettingRepository(fakeClient);

        var actual = await repository.GetAwcCockfightConfigurationAsync().ConfigureAwait(false);

        Assert.IsNotNull(actual);
        expected.ToExpectedObject().ShouldEqual(actual);
    }

    [TestMethod]
    public async Task Get_Awc_Facait_Configuration()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();

        var key = GameProviderConfigurations.AWC_FACAI;

        var expected = new AwcFacaiConfiguration()
        {
            ApiUrl = new Uri("http://apiurl.google.com"),
            ApiRecordUrl = new Uri("http://apirecordurl.google.com")
        };
        _ = fakeClient.SettingGetConfigurationAsync<AwcFacaiConfiguration>(Arg.Is(key)).Returns(expected);

        var repository = new SettingRepository(fakeClient);

        var actual = await repository.GetAwcFacaiConfigurationAsync().ConfigureAwait(false);

        Assert.IsNotNull(actual);
        expected.ToExpectedObject().ShouldEqual(actual);
    }

    [TestMethod]
    public async Task Get_Awc_Fs_Configuration()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();

        var key = GameProviderConfigurations.AWC_FS;

        var expected = new AwcFsConfiguration()
        {
            ApiUrl = new Uri("http://apiurl.google.com"),
            ApiRecordUrl = new Uri("http://apirecordurl.google.com")
        };
        _ = fakeClient.SettingGetConfigurationAsync<AwcFsConfiguration>(Arg.Is(key)).Returns(expected);

        var repository = new SettingRepository(fakeClient);

        var actual = await repository.GetAwcFsConfigurationAsync().ConfigureAwait(false);

        Assert.IsNotNull(actual);
        expected.ToExpectedObject().ShouldEqual(actual);
    }

    [TestMethod]
    public async Task Get_Awc_Km_Configuration()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();

        var key = GameProviderConfigurations.AWC_KM;

        var expected = new AwcKmConfiguration()
        {
            ApiUrl = new Uri("http://apiurl.google.com"),
            ApiRecordUrl = new Uri("http://apirecordurl.google.com")
        };
        _ = fakeClient.SettingGetConfigurationAsync<AwcKmConfiguration>(Arg.Is(key)).Returns(expected);

        var repository = new SettingRepository(fakeClient);

        var actual = await repository.GetAwcKmConfigurationAsync().ConfigureAwait(false);

        Assert.IsNotNull(actual);
        expected.ToExpectedObject().ShouldEqual(actual);
    }

    [TestMethod]
    public async Task Get_Awc_Ludo_Configuration()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();

        var key = GameProviderConfigurations.AWC_LUDO;

        var expected = new AwcLudoConfiguration()
        {
            ApiUrl = new Uri("http://apiurl.google.com"),
            ApiRecordUrl = new Uri("http://apirecordurl.google.com")
        };
        _ = fakeClient.SettingGetConfigurationAsync<AwcLudoConfiguration>(Arg.Is(key)).Returns(expected);

        var repository = new SettingRepository(fakeClient);

        var actual = await repository.GetAwcLudoConfigurationAsync().ConfigureAwait(false);

        Assert.IsNotNull(actual);
        expected.ToExpectedObject().ShouldEqual(actual);
    }

    [TestMethod]
    public async Task Get_Awc_Rcb_Configuration()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();

        var key = GameProviderConfigurations.AWC_RCB;

        var expected = new AwcRcbConfiguration()
        {
            ApiUrl = new Uri("http://apiurl.google.com"),
            ApiRecordUrl = new Uri("http://apirecordurl.google.com")
        };
        _ = fakeClient.SettingGetConfigurationAsync<AwcRcbConfiguration>(Arg.Is(key)).Returns(expected);

        var repository = new SettingRepository(fakeClient);

        var actual = await repository.GetAwcRcbConfigurationAsync().ConfigureAwait(false);

        Assert.IsNotNull(actual);
        expected.ToExpectedObject().ShouldEqual(actual);
    }

    [TestMethod]
    public async Task Get_Awc_Rgs_Configuration()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();

        var key = GameProviderConfigurations.AWC_PGS;

        var expected = new AwcPgsConfiguration()
        {
            ApiUrl = new Uri("http://apiurl.google.com"),
            ApiRecordUrl = new Uri("http://apirecordurl.google.com")
        };
        _ = fakeClient.SettingGetConfigurationAsync<AwcPgsConfiguration>(Arg.Is(key)).Returns(expected);

        var repository = new SettingRepository(fakeClient);

        var actual = await repository.GetAwcPgsConfigurationAsync().ConfigureAwait(false);

        Assert.IsNotNull(actual);
        expected.ToExpectedObject().ShouldEqual(actual);
    }

    [TestMethod]
    public async Task Get_Awc_Sg_Configuration()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();

        var key = GameProviderConfigurations.AWC_SG;

        var expected = new AwcSgConfiguration()
        {
            ApiUrl = new Uri("http://apiurl.google.com"),
            ApiRecordUrl = new Uri("http://apirecordurl.google.com")
        };
        _ = fakeClient.SettingGetConfigurationAsync<AwcSgConfiguration>(Arg.Is(key)).Returns(expected);

        var repository = new SettingRepository(fakeClient);

        var actual = await repository.GetAwcSgConfigurationAsync().ConfigureAwait(false);

        Assert.IsNotNull(actual);
        expected.ToExpectedObject().ShouldEqual(actual);
    }

    [TestMethod]
    public async Task Get_Awc_Spade_Configuration()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();

        var key = GameProviderConfigurations.AWC_SPADE;

        var expected = new AwcSpadeConfiguration()
        {
            ApiUrl = new Uri("http://apiurl.google.com"),
            ApiRecordUrl = new Uri("http://apirecordurl.google.com")
        };
        _ = fakeClient.SettingGetConfigurationAsync<AwcSpadeConfiguration>(Arg.Is(key)).Returns(expected);

        var repository = new SettingRepository(fakeClient);

        var actual = await repository.GetAwcSpadeConfigurationAsync().ConfigureAwait(false);

        Assert.IsNotNull(actual);
        expected.ToExpectedObject().ShouldEqual(actual);
    }

    [TestMethod]
    public async Task Get_Awc_Venus_Configuration()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();

        var key = GameProviderConfigurations.AWC_VENUS;

        var expected = new AwcVenusConfiguration()
        {
            ApiUrl = new Uri("http://apiurl.google.com"),
            ApiRecordUrl = new Uri("http://apirecordurl.google.com")
        };
        _ = fakeClient.SettingGetConfigurationAsync<AwcVenusConfiguration>(Arg.Is(key)).Returns(expected);

        var repository = new SettingRepository(fakeClient);

        var actual = await repository.GetAwcVenusConfigurationAsync().ConfigureAwait(false);

        Assert.IsNotNull(actual);
        expected.ToExpectedObject().ShouldEqual(actual);
    }

    [TestMethod]
    public async Task Get_Awc_Yl_Configuration()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();

        var key = GameProviderConfigurations.AWC_YL;

        var expected = new AwcYlConfiguration()
        {
            ApiUrl = new Uri("http://apiurl.google.com"),
            ApiRecordUrl = new Uri("http://apirecordurl.google.com")
        };
        _ = fakeClient.SettingGetConfigurationAsync<AwcYlConfiguration>(Arg.Is(key)).Returns(expected);

        var repository = new SettingRepository(fakeClient);

        var actual = await repository.GetAwcYlConfigurationAsync().ConfigureAwait(false);

        Assert.IsNotNull(actual);
        expected.ToExpectedObject().ShouldEqual(actual);
    }

    [TestMethod]
    public async Task Get_AwcAws_Configuration()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();

        var key = GameProviderConfigurations.AWC_AWS;

        var expected = new AwcAwsConfiguration()
        {
            ApiUrl = new Uri("http://apiurl.google.com"),
            ApiRecordUrl = new Uri("http://apirecordurl.google.com")
        };
        _ = fakeClient.SettingGetConfigurationAsync<AwcAwsConfiguration>(Arg.Is(key)).Returns(expected);

        var repository = new SettingRepository(fakeClient);

        var actual = await repository.GetAwcAwsConfigurationAsync().ConfigureAwait(false);

        Assert.IsNotNull(actual);
        expected.ToExpectedObject().ShouldEqual(actual);
    }

    [TestMethod]
    public async Task Get_Configuration()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();

        var key = GameProviderConfigurations.KY;

        var expected = new KYConfiguration()
        {
            ServiceEndpointUrl = new("http://game-ky"),
            AmountMaximum = 10000000,
            AmountFormat = "10,2",
            BrandMerchants = new KYBrandMerchant[] { new KYBrandMerchant()
            {
                BrandCode = "Default",
                ApiUrl = new("https://wc1-api.hddv1.com/channelHandle"),
                ApiRecordUrl = new("https://wc1-record.hddv1.com/getRecordHandle"),
                Deskey = "E2F3A416006F84F8",
                Md5key = "B563DF2A3DE58DFB",
                AgentMappers = new KYAgentMapper[] {
                    new KYAgentMapper() {
                        CurrencyCode = "CNY",
                        AgentId = "72412",
                        Status = true
                    }
                },
                Status = true
            } }
        };
        _ = fakeClient.SettingGetConfigurationAsync<KYConfiguration>(Arg.Is(key)).Returns(expected);

        var repository = new SettingRepository(fakeClient);

        var actual = await repository.GetConfigurationAsync<KYConfiguration>(key).ConfigureAwait(false);

        Assert.IsNotNull(actual);
        expected.ToExpectedObject().ShouldEqual(actual);
    }
}
