using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.SM01Service.Client;
using Tgs.GameInterface.SM01Service.Protos;
using Tgs.GameProviders.Core.SM01.ApplicationServices;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.SettingConfiguration;

namespace Tgs.GameProviders.Core.UnitTest.SM01;

[TestFixture]
public class SM01PlatformLoginGetServiceUnitTest
{
    [Test]
    public async Task Normal_Process()
    {
        var fakeBrandService = Substitute.For<IBrandRepository>();
        var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
        var fakePlayerRepository = Substitute.For<IPlayerRepository>();
        var fakeGameInterfaceSM01Client = Substitute.For<IGameInterfaceSM01Client>();

        var query = new SM01PlatformLoginQuery
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

        var response = new PlatformLoginReply()
        {
            MerchantId = "SQjCxm2cX5bahBta",
            CustomerName = "4ucmqplxb029",
            Timestamp = 1670390195123,
            Sign = "46C9EA4F03900B74379A15A56ED14229"
        };
        _ = fakeGameInterfaceSM01Client.PlatformLoginAsync(
                Arg.Is(configuration.ApiUrl),
                Arg.Is(configuration.MerchantId),
                Arg.Is(configuration.MerchantKey),
                Arg.Is(player.PlayerCode),
                Arg.Any<CancellationToken>()).Returns(response);

        var sut = new SM01PlatformLoginGetService(
            fakeBrandService,
            fakeGameProviderRepository,
            fakePlayerRepository,
            fakeGameInterfaceSM01Client);

        var actual = await sut.GetAsync(query).ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.AreEqual(response.MerchantId, actual.MerchantId);
        Assert.AreEqual(response.CustomerName, actual.CustomerName);
        Assert.AreEqual(response.Timestamp, actual.Timestamp);
        Assert.AreEqual(response.Sign, actual.Sign);
    }
}
