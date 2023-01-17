using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.SkywindService.Client;
using Tgs.GameInterface.SkywindService.Protos;
using Tgs.GameProviders.Core.SkywindTransfer.ApplicationServices;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.UnitTest.SkywindTransfer;

[TestFixture]
public class SkywindTransferPlayerBalanceGetServiceUnitTest
{
    [Test]
    public async Task Normal_Process()
    {
        var fakeCreatePlayerCommandService = Substitute.For<ICommandService<SkywindTransferCreatePlayerCommand>>();
        var fakeGameInterfaceSkywindClient = Substitute.For<IGameInterfaceSkywindClient>();
        var fakePlayerRepository = Substitute.For<IPlayerRepository>();
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

        var playerModel = new PlayerInfo(
            ResellerId: Guid.NewGuid(),
            ResellerCode: "GPI",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "GPI",
            BrandId: Guid.NewGuid(),
            BrandCode: "GPI",
            PlayerCode: "4ucmqplxb029",
            Username: "4ucmqplxb029",
            DefaultWalletId: Guid.NewGuid(),
            DefaultWalletCurrencyCode: "CNY");

        var query = new SkywindTransferPlayerBalanceQuery
        {
            PlayerCode = "4ucmqplxb029"
        };

        var gameProviderConfigurationId = Guid.NewGuid();
        var gameProviderId = Guid.NewGuid();

        _ = fakeSettingRepository.GetSkywindTransferConfigurationAsync().Returns(ValueTask.FromResult(configuration));
        _ = fakePlayerRepository.GetAsync(Arg.Is(query.PlayerCode)).Returns(playerModel);

        var request = new PlayerBalanceRequest()
        {
            ApiUrl = configuration.ApiUrl.ToString(),
            SecretKey = configuration.SecretKey,
            Username = configuration.Username,
            Password = configuration.Password,
            PlayerCode = query.PlayerCode
        };
        var response = new PlayerBalanceReply()
        {
            IsPlayer = true,
            Balance = 100m
        };
        _ = fakeGameInterfaceSkywindClient.PlayerBalanceAsync(
            Arg.Is(request)).Returns(response);

        var sut = new SkywindTransferPlayerBalanceGetService(
             fakeCreatePlayerCommandService,
             fakeGameInterfaceSkywindClient,
             fakePlayerRepository,
             fakeSettingRepository);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.AreEqual(response.Balance.ToDecimal(), actual.Value);
    }
}
