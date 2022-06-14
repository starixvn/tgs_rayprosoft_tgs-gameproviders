using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameInterface.AwcService.Client;
using Tgs.GameInterface.AwcService.Protos;
using Tgs.GameProviders.Core.AwcAws.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Core.UnitTest.AwcAws;

[TestFixture]
public class AwcAwsPlayerFundOutGetServiceUnitTest
{
    [Test]
    public async Task Normal_Process()
    {
        var fakeBrandService = Substitute.For<IBrandService>();
        var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
        var fakeGameInterfaceAwcServiceGrpc = Substitute.For<IGameInterfaceAwcServiceGrpc>();
        var fakePlayerRepository = Substitute.For<IPlayerRepository>();
        var fakeSettingRepository = Substitute.For<ISettingRepository>();

        var gameUrl = new Uri("http://www.google.com");

        var configuration = new AwcAwsConfiguration
        {
            ApiUrl = new("https://tttint.onlinegames22.com/"),
            ApiRecordUrl = new("https://tttint.onlinegames22.com/")
        };

        var configurationData = new AwcConfigurationData(
            AgentId: "thbgpiaelotto",
            Cert: "sJQKnLeKugNHE7yqW7C",
            BetLimit: null);

        var playerModel = new PlayerModel(
            ResellerId: Guid.NewGuid(),
            ResellerCode: "GPI",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "GPI",
            BrandId: Guid.NewGuid(),
            BrandCode: "GPI",
            PlayerCode: "4ucmqplxb029",
            Username: "4ucmqplxb029",
            DefaultWalletId: Guid.NewGuid(),
            DefaultWalletCurrencyCode: "CNY",
            PlayerStatus.Enabled);

        var query = new AwcAwsPlayerFundOutQuery
        {
            BrandCode = "GPI",
            PlayerCode = "4ucmqplxb029",
            CurrencyCode = "CNY",
            OrderId = Guid.NewGuid().ToString("N"),
            Amount = 100m
        };

        var gameProviderConfigurationId = Guid.NewGuid();
        var gameProviderId = Guid.NewGuid();
        _ = fakeGameProviderRepository.GetGameProviderIdAsync(Arg.Is(GameProviderType.AWC_AWS)).Returns(gameProviderId);
        _ = fakeSettingRepository.GetAwcAwsConfigurationAsync().Returns(ValueTask.FromResult(configuration)); ;
        _ = fakePlayerRepository.GetAsync(Arg.Is(query.PlayerCode)).Returns(playerModel);
        _ = fakeBrandService.GetBrandGameProviderConfigurationIdAsync(
             Arg.Is(gameProviderId),
             Arg.Is(playerModel.BrandId)).Returns(gameProviderConfigurationId);
        _ = fakeGameProviderRepository.GetAwcConfigurationData(Arg.Is(gameProviderConfigurationId)).Returns(configurationData);

        var gameProductTransactionId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
        var reply = new GameInterfaceAwcServiceFundReply()
        {
            Success = true,
            TransactionId = query.OrderId,
            GameProductTransactionId = gameProductTransactionId,
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

        var sut = new AwcAwsPlayerFundOutGetService(
            fakeBrandService,
            fakeGameProviderRepository,
            fakeGameInterfaceAwcServiceGrpc,
            fakePlayerRepository,
            fakeSettingRepository);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.AreEqual(gameProductTransactionId, actual.OrderId);
    }
}
