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
