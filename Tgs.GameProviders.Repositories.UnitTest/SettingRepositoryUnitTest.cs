using ExpectedObjects;
using NSubstitute;
using Tgs.GameCommand.Client;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Repositories.UnitTest;

public class SettingRepositoryUnitTest
{
    [Fact]
    public async Task Get_Configuration()
    {
        var fakeClient = Substitute.For<IGameCommandClient>();

        var key = GameProviderConfigurations.Kaiyuan;

        var expected = new KaiyuanConfiguration()
        {
            AmountMaximum = 10000000,
            AmountFormat = "10,2",
            ApiUrl = new("https://wc1-api.hddv1.com/"),
            ApiRecordUrl = new("https://wc1-record.hddv1.com/"),
            Deskey = "E2F3A416006F84F8",
            Md5key = "B563DF2A3DE58DFB",
            CurrencyCode = "CNY",
            AgentId = "72412"
        };
        _ = fakeClient.SettingGetConfigurationAsync<KaiyuanConfiguration>(Arg.Is(key)).Returns(expected);

        var repository = new SettingRepository(fakeClient);

        var actual = await repository.GetConfigurationAsync<KaiyuanConfiguration>(key).ConfigureAwait(false);

        Assert.NotNull(actual);
        expected.ToExpectedObject().ShouldEqual(actual);
    }
}
