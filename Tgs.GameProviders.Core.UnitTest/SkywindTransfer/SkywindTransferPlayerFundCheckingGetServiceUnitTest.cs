using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameInterface.SkywindService.Client;
using Tgs.GameInterface.SkywindService.Protos;
using Tgs.GameProviders.Core.SkywindTransfer.ApplicationServices;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.SkywindTransfer;

[TestFixture]
public class SkywindTransferPlayerFundCheckingGetServiceUnitTest
{
    [Test]
    public async Task Normal_Process()
    {
        var fakeGameInterfaceSkywindClient = Substitute.For<IGameInterfaceSkywindClient>();
        var fakeSettingRepository = Substitute.For<ISettingRepository>();

        var configuration = new SkywindTransferConfiguration
        {
            ApiUrl = new("http://localhost/"),
            LiveLobbyUrl = new("http://localhost/"),
            MerchId = "merchId",
            MerchPwd = "merchPwd",
            SecretKey = "secretKey",
            Username = "username",
            Password = "password"
        };

        var query = new SkywindTransferPlayerFundCheckingQuery
        {
            BrandCode = "GPI",
            PlayerCode = "test",
            CurrencyCode = "CNY",
            OrderId = Guid.NewGuid().ToString("N")
        };

        _ = fakeSettingRepository.GetSkywindTransferConfigurationAsync().Returns(ValueTask.FromResult(configuration));

        var request = new FundStatusRequest()
        {
            ApiUrl = configuration.ApiUrl.ToString(),
            SecretKey = configuration.SecretKey,
            Username = configuration.Username,
            Password = configuration.Password,
            ExtTrxId = query.OrderId
        };
        var response = new FundStatusReply()
        {
            IsTransaction = true,
            Status = "committed"
        };
        _ = fakeGameInterfaceSkywindClient.FundStatusAsync(
            Arg.Is(request)).Returns(response);

        var sut = new SkywindTransferPlayerFundCheckingGetService(
             fakeGameInterfaceSkywindClient,
             fakeSettingRepository);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.IsTrue(actual.OrderStatus);
    }
}
