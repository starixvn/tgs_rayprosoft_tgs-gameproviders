using System;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.SM01Service.Client;
using Tgs.GameInterface.SM01Service.Protos;
using Tgs.GameProviders.Core.SM01.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.SM01;

public class SM01PlayerFundOutGetServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var fakeBrandService = Substitute.For<IBrandRepository>();
        var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
        var fakeGameInterfaceSM01Client = Substitute.For<IGameInterfaceSM01Client>();
        var fakePlayerRepository = Substitute.For<IPlayerRepository>();

        var query = new SM01PlayerFundOutQuery
        {
            BrandCode = "GPI",
            PlayerCode = "4ucmqplxb029",
            CurrencyCode = "CNY",
            OrderId = Guid.NewGuid().ToString("N"),
            Amount = 100m
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

        var reply = new FundReply()
        {
            RefTransactionCode = query.OrderId,
            TransactionId = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
            Balance = 100,
        };

        _ = fakeGameInterfaceSM01Client.FundOutAsync(
                Arg.Is(configuration.ApiUrl),
                Arg.Is(configuration.MerchantId),
                Arg.Is(configuration.MerchantKey),
                Arg.Is(player.PlayerCode),
                Arg.Is(query.OrderId),
                Arg.Is(int.Parse(query.Amount.ToString()))).Returns(reply);

        var sut = new SM01PlayerFundOutGetService(
            fakeBrandService,
            fakeGameProviderRepository,
            fakeGameInterfaceSM01Client,
            fakePlayerRepository);

        var actual = await sut.GetAsync(query);

        Assert.NotNull(actual);
    }
}
