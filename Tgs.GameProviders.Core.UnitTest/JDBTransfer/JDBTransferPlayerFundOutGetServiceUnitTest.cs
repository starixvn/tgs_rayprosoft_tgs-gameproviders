using System;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.JDBService.Client;
using Tgs.GameInterface.JDBService.Protos;
using Tgs.GameProviders.Core.JDBTransfer.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.JDBTransfer;

public class JDBTransferPlayerFundOutGetServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var fakeGameInterfaceJDBClient = Substitute.For<IGameInterfaceJDBClient>();
        var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
        var fakePlayerRepository = Substitute.For<IPlayerRepository>();
        var fakeBrandService = Substitute.For<IBrandRepository>();

        var query = new JDBTransferPlayerFundOutQuery
        {
            BrandCode = "GPI",
            PlayerCode = "4ucmqplxb029",
            CurrencyCode = "CNY",
            OrderId = Guid.NewGuid().ToString("N"),
            Amount = 100m
        };

        var gameProviderId = Guid.NewGuid();
        _ = fakeGameProviderRepository.GetGameProviderIdAsync(Arg.Is(GameProviderType.JDBTransfer)).Returns(gameProviderId);

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

        var configuration = new JDBTransferConfiguration()
        {
            ApiUrl = new("http://localhost/"),
            Domain = "Domain",
            Agent = "Agent",
            AesIV = "AesIV",
            AesKey = "AesKey"
        };
        _ = fakeGameProviderRepository.GetJDBTransferConfigurationAsync(Arg.Is(gameProviderConfigurationId)).Returns(configuration);

        var reply = new FundReply()
        {
            SerialNo = query.OrderId,
            Resp = new JsonObject { }.ToJsonString()
        };
        _ = fakeGameInterfaceJDBClient.FundOutAsync(
                Arg.Is(configuration.ApiUrl),
                Arg.Is(configuration.Domain),
                Arg.Is(configuration.AesKey),
                Arg.Is(configuration.AesIV),
                Arg.Is(configuration.Agent),
                Arg.Is(player.PlayerCode),
                Arg.Is(query.OrderId),
                Arg.Is(query.Amount)).Returns(reply);

        var sut = new JDBTransferPlayerFundOutGetService(
            fakeGameInterfaceJDBClient,
            fakeGameProviderRepository,
            fakePlayerRepository,
            fakeBrandService);

        var actual = await sut.GetAsync(query);

        Assert.NotNull(actual);
        Assert.Equal(query.OrderId, actual.OrderId);
    }
}
