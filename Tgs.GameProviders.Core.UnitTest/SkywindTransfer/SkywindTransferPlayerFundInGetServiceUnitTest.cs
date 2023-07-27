using System;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameInterface.SkywindService.Client;
using Tgs.GameInterface.SkywindService.Protos;
using Tgs.GameProviders.Core.SkywindTransfer.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.SkywindTransfer;

public class SkywindTransferPlayerFundInGetServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
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

        var query = new SkywindTransferPlayerFundInQuery
        {
            BrandCode = "GPI",
            PlayerCode = "test",
            CurrencyCode = "CNY",
            OrderId = Guid.NewGuid().ToString("N"),
            Amount = 1
        };
        var gameProviderId = Guid.NewGuid();
        _ = fakeGameProviderRepository.GetGameProviderIdAsync(Arg.Is(GameProviderType.SkywindTransfer)).Returns(ValueTask.FromResult(gameProviderId));

        _ = fakeSettingRepository.GetSkywindTransferConfigurationAsync().Returns(ValueTask.FromResult(configuration));

        var gameProviderCurrencyCode = "CNY";
        _ = fakeGameProviderRepository
            .GetGameProviderCurrencyCodeAsync(
                Arg.Is(gameProviderId),
                Arg.Is(query.CurrencyCode))
            .Returns(ValueTask.FromResult(gameProviderCurrencyCode));

        var request = new FundRequest()
        {
            ApiUrl = configuration.ApiUrl.ToString(),
            SecretKey = configuration.SecretKey,
            Username = configuration.Username,
            Password = configuration.Password,
            ExtTrxId = query.OrderId,
            PlayerCode = query.PlayerCode,
            Currency = gameProviderCurrencyCode,
            Amount = query.Amount,
        };
        var response = new FundReply()
        {
            Status = true,
            TrxId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(),
            ExtTrxId = query.OrderId,
            OrderStatus = "approved",
            Amount = query.Amount,
            Balance = 0
        };
        _ = fakeGameInterfaceSkywindClient.FundInAsync(
            Arg.Is(request)).Returns(response);

        var sut = new SkywindTransferPlayerFundInGetService(
            fakeGameProviderRepository,
             fakeGameInterfaceSkywindClient,
             fakeSettingRepository);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.Equal(query.OrderId, actual.OrderId);
    }
}
