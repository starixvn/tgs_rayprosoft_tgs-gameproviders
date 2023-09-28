using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.SM01Service.Client;
using Tgs.GameInterface.SM01Service.Protos;
using Tgs.GameProviders.Core.SM01.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.SM01;

public class SM01PlayerBalanceGetServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var fakeBrandService = Substitute.For<IBrandRepository>();
        var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
        var fakeGameInterfaceSM01Client = Substitute.For<IGameInterfaceSM01Client>();
        var fakePlayerRepository = Substitute.For<IPlayerRepository>();

        var query = new SM01PlayerBalanceQuery
        {
            PlayerCode = "4ucmqplxb029"
        };

        var gameProviderId = Guid.NewGuid();
        _ = fakeGameProviderRepository.GetGameProviderIdAsync(Arg.Is(GameProviderType.SM01)).Returns(gameProviderId);

        var player = new PlayerInfo(
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
        _ = fakePlayerRepository.GetAsync(Arg.Is(query.PlayerCode)).Returns(player);

        var gameProviderConfigurationId = Guid.NewGuid();
        _ = fakeBrandService.GetBrandGameProviderConfigurationIdAsync(
             Arg.Is(gameProviderId),
             Arg.Is(player.BrandId)).Returns(gameProviderConfigurationId);

        var configuration = new SM01Configuration()
        {
            ApiUrl = new("https://cn.ucfclive.com"),
            MerchantId = "SQjCxm2cX5bahBta",
            MerchantKey = "XGKGkcmYNnfex8AecKzCEbD4GMExA2cb",
        };
        _ = fakeGameProviderRepository.GetSM01ConfigurationAsync(Arg.Is(gameProviderConfigurationId)).Returns(configuration);

        var response = new PlayerBalanceReply()
        {
            IsPlayerExist = true,
            Balance = 1000
        };
        _ = fakeGameInterfaceSM01Client.PlayerBalanceAsync(
                Arg.Is(configuration.ApiUrl),
                Arg.Is(configuration.MerchantId),
                Arg.Is(configuration.MerchantKey),
                Arg.Is(player.PlayerCode),
                Arg.Any<CancellationToken>()).Returns(response);

        var sut = new SM01PlayerBalanceGetService(
            fakeBrandService,
            fakeGameProviderRepository,
            fakeGameInterfaceSM01Client,
            fakePlayerRepository);

        var actual = await sut.GetAsync(query);

        Assert.NotNull(actual);
        Assert.Equal(response.Balance.ToDecimal(), actual.Balance);
    }
}
