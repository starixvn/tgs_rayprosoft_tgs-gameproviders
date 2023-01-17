using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.AwcService.Client;
using Tgs.GameInterface.AwcService.Protos;
using Tgs.GameProviders.Core.AwcFacai.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.ConfigurationData;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.AwcFacai;

[TestFixture]
public class AwcFacaiPlayerFundOutGetServiceUnitTest
{
    [Test]
    public async Task Normal_Process()
    {
        var fakeBrandService = Substitute.For<IBrandRepository>();
        var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
        var fakeGameInterfaceAwcServiceGrpc = Substitute.For<IGameInterfaceAwcServiceGrpc>();
        var fakePlayerRepository = Substitute.For<IPlayerRepository>();
        var fakeSettingRepository = Substitute.For<ISettingRepository>();

        var configuration = new AwcFacaiConfiguration
        {
            ApiUrl = new("https://tttint.onlinegames22.com/"),
            ApiRecordUrl = new("https://tttint.onlinegames22.com/")
        };

        var configurationData = new AwcConfigurationData(
            AgentId: "testAgentId",
            Cert: "testCert",
            BetLimit: null);

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

        var query = new AwcFacaiPlayerFundOutQuery
        {
            BrandCode = "GPI",
            PlayerCode = "4ucmqplxb029",
            CurrencyCode = "CNY",
            OrderId = Guid.NewGuid().ToString("N"),
            Amount = 100m
        };

        var gameProviderConfigurationId = Guid.NewGuid();
        var gameProviderId = Guid.NewGuid();
        _ = fakeGameProviderRepository.GetGameProviderIdAsync(Arg.Is(GameProviderType.AWC_FACAI)).Returns(gameProviderId);
        _ = fakeSettingRepository.GetAwcFacaiConfigurationAsync().Returns(ValueTask.FromResult(configuration)); ;
        _ = fakePlayerRepository.GetAsync(Arg.Is(query.PlayerCode)).Returns(playerModel);
        _ = fakeBrandService.GetBrandGameProviderConfigurationIdAsync(
             Arg.Is(gameProviderId),
             Arg.Is(playerModel.BrandId)).Returns(gameProviderConfigurationId);
        _ = fakeGameProviderRepository.GetAwcConfigurationData(Arg.Is(gameProviderConfigurationId)).Returns(configurationData);

        var gameProviderTransactionId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
        var reply = new GameInterfaceAwcServiceFundReply()
        {
            Success = true,
            TransactionId = query.OrderId,
            GameProviderTransactionId = gameProviderTransactionId,
            CurrentBalance = 100m,
            Amount = query.Amount
        };
        _ = fakeGameInterfaceAwcServiceGrpc.FundOutAsync(
            Arg.Is(configuration.ApiUrl),
            Arg.Is(configurationData.AgentId),
            Arg.Is(configurationData.Cert),
            Arg.Is(query.PlayerCode),
            Arg.Is(query.OrderId),
            Arg.Is(query.Amount)).Returns(reply);

        var sut = new AwcFacaiPlayerFundOutGetService(
            fakeBrandService,
            fakeGameProviderRepository,
            fakeGameInterfaceAwcServiceGrpc,
            fakePlayerRepository,
            fakeSettingRepository);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.AreEqual(gameProviderTransactionId, actual.OrderId);
    }
}
