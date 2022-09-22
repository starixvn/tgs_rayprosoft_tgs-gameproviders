using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameInterface.SBOService.Client;
using Tgs.GameInterface.SBOService.Protos;
using Tgs.GameProviders.Core.SBOTransfer.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Core.UnitTest.SBOTransfer;

[TestFixture]
public class SBOTransferPlayerFundOutGetServiceUnitTest
{
    [Test]
    public async Task Normal_Process()
    {
        var fakeGameInterfaceSBOClient = Substitute.For<IGameInterfaceSBOClient>();
        var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
        var fakePlayerRepository = Substitute.For<IPlayerRepository>();
        var fakeBrandService = Substitute.For<IBrandRepository>();

        var query = new SBOTransferPlayerFundOutQuery
        {
            BrandCode = "GPI",
            PlayerCode = "4ucmqplxb029",
            CurrencyCode = "CNY",
            OrderId = Guid.NewGuid().ToString("N"),
            Amount = 100m
        };

        var gameProviderId = Guid.NewGuid();
        _ = fakeGameProviderRepository.GetGameProviderIdAsync(Arg.Is(GameProviderType.SBOTransfer)).Returns(gameProviderId);

        var player = new PlayerModel(
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
        _ = fakePlayerRepository.GetAsync(Arg.Is(query.PlayerCode)).Returns(player);

        var gameProviderConfigurationId = Guid.NewGuid();
        _ = fakeBrandService.GetBrandGameProviderConfigurationIdAsync(
             Arg.Is(gameProviderId),
             Arg.Is(player.BrandId)).Returns(gameProviderConfigurationId);

        var configuration = new SBOTransferConfiguration()
        {
            ApiUrl = new("http://localhost/"),
            CompanyKey = "companyKey",
            Agent = "Agent",
            Currency = "CNY",
            Min = 1,
            Max = 5000,
            MaxPerMatch = 20000,
            CasinoTableLimit = 1
        };
        _ = fakeGameProviderRepository.GetSBOTransferConfiguration(Arg.Is(gameProviderConfigurationId)).Returns(configuration);

        var reply = new FundReply()
        {
            Refno = Guid.NewGuid().ToString("N"),
            TxnId = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
            Balance = 100,
        };
        _ = fakeGameInterfaceSBOClient.FundOutAsync(
                Arg.Is(configuration.ApiUrl),
                Arg.Is(configuration.CompanyKey),
                Arg.Is(configuration.Agent),
                Arg.Is(player.PlayerCode),
                Arg.Is(query.Amount)).Returns(reply);

        var sut = new SBOTransferPlayerFundOutGetService(
            fakeGameInterfaceSBOClient,
            fakeGameProviderRepository,
            fakePlayerRepository,
            fakeBrandService);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
    }
}
