using System;
using System.Text.Json;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameInterface.AwcService.Client;
using Tgs.GameInterface.AwcService.Protos;
using Tgs.GameProviders.Core.AwcVenus.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.QueryServer.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.UnitTest.AwcVenus;

[TestFixture]
public class AwcVenusPlayerBalanceGetServiceUnitTest
{
    [Test]
    public async Task Normal_Process()
    {
        var fakeBrandService = Substitute.For<IBrandService>();
        var fakeCreatePlayerCommandService = Substitute.For<ICommandService<AwcVenusCreatePlayerCommand>>();
        var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
        var fakeGameInterfaceAwcServiceGrpc = Substitute.For<IGameInterfaceAwcServiceGrpc>();
        var fakePlayerRepository = Substitute.For<IPlayerRepository>();
        var fakeSettingRepository = Substitute.For<ISettingRepository>();

        var configuration = new AwcVenusConfiguration
        {
            ApiUrl = new("https://tttint.onlinegames22.com/"),
            ApiRecordUrl = new("https://tttint.onlinegames22.com/")
        };

        var configurationData = new AwcConfigurationData(
            AgentId: "testAgentId",
            Cert: "testCert",
            BetLimit: JsonDocument.Parse(JsonSerializer.Serialize(new
            {
                VENUS = new
                {
                    LIVE = new
                    {
                        limitId = new[] { "280303", "280304", "280305" }
                    }
                }
            })));

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

        var query = new AwcVenusPlayerBalanceQuery
        {
            PlayerCode = "4ucmqplxb029"
        };

        var gameProviderConfigurationId = Guid.NewGuid();
        var gameProviderId = Guid.NewGuid();
        _ = fakeGameProviderRepository.GetGameProviderIdAsync(Arg.Is(GameProviderType.AWC_VENUS)).Returns(gameProviderId);
        _ = fakeSettingRepository.GetAwcVenusConfigurationAsync().Returns(ValueTask.FromResult(configuration));
        _ = fakePlayerRepository.GetAsync(Arg.Is(query.PlayerCode)).Returns(playerModel);
        _ = fakeBrandService.GetBrandGameProviderConfigurationIdAsync(
             Arg.Is(gameProviderId),
             Arg.Is(playerModel.BrandId)).Returns(gameProviderConfigurationId);
        _ = fakeGameProviderRepository.GetAwcConfigurationData(Arg.Is(gameProviderConfigurationId)).Returns(configurationData);

        var response = new GameInterfaceAwcServiceBalanceReply()
        {
            Success = true,
            IsPlayerExist = true,
            Balance = 100m
        };
        _ = fakeGameInterfaceAwcServiceGrpc.GetBalanceAsync(
            Arg.Is(configuration.ApiUrl),
            Arg.Is(configurationData.AgentId),
            Arg.Is(configurationData.Cert),
            Arg.Is(query.PlayerCode)).Returns(response);

        var sut = new AwcVenusPlayerBalanceGetService(
            fakeBrandService,
            fakeCreatePlayerCommandService,
            fakeGameProviderRepository,
            fakeGameInterfaceAwcServiceGrpc,
            fakePlayerRepository,
            fakeSettingRepository);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.AreEqual(response.Balance.ToDecimal(), actual.Value);
    }
}
