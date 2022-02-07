using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using TGS.GameProviders.Core.Jili;
using TGS.GameProviders.Core.Jili.ApplicationServices;
using TGS.GameProviders.Domains.SettingConfiguration;
using TGS.GameProviders.Domains.SettingConfiguration.Jili;

namespace TGS.GameProviders.Core.UnitTests.Jili;

[TestFixture]
internal class JiliKickPlayerCommandServiceUnitTest
{
    [Test]
    public async Task Normal_Process()
    {
        var faceLogger = NullLogger<JiliKickPlayerCommandService>.Instance;
        var fakeApiClient = Substitute.For<IJiliApiClient>();
        var fakeService = Substitute.For<IJiliService>();
        var fakeMerchantMapper = Substitute.For<IJiliMerchantMapper>();
        var fakePlayerService = Substitute.For<IPlayerService>();

        var command = new JiliKickPlayerCommand
        {
            PlayerCode = "4ucmqplxb029"
        };

        var playerModel = new PlayerInfo(
            Guid.Parse("b5a2c708-ab70-4316-9f4d-5d2ad0e77460"),
            "TGS",
            Guid.Parse("66c5fef5-217b-4929-9a08-72c2479a8f4e"),
            "TGS",
            Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af"),
            "GPI",
            "4ucmqplxb029",
            "4ucmqplxb029",
            Guid.Parse("36a6cdef-094f-eb11-8d88-70b5e82b6b8d"),
            "CNY",
            PlayerStatus.Enabled);

        var merchant = new JiliMerchant
        {
            BrandCode = "Default",
            AgentId = "PG_Seamless",
            AgentKey = "0d90dda822b6df548460da8f551c1a76c01b6a9e"
        };

        var configuration = new JiliConfiguration
        {
            ApiUrl = new Uri("https://uat-wb-api.jlfafafa2.com"),
            Merchants = new[]
            {
                merchant
            },
            ClientId = "ClientId",
            SecretKey = "SecretKey"
        };

        var sut = new JiliKickPlayerCommandService(
            faceLogger,
            fakePlayerService,
            fakeService,
            fakeMerchantMapper,
            fakeApiClient);

        _ = fakeService.GetSettingConfigurationAsync().Returns(ValueTask.FromResult(configuration));
        _ = fakeMerchantMapper.GetMerchant(configuration.Merchants, playerModel.BrandCode).Returns(merchant);
        _ = fakePlayerService.GetCacheAsync(command.PlayerCode).Returns(Task.FromResult(playerModel));

        await sut.ExecuteAsync(command).ConfigureAwait(false);

        _ = fakeApiClient.Received(1)
            .KickMemberAsync(
                configuration.ApiUrl,
                $"{playerModel.BrandCode}_{playerModel.PlayerCode}",
                merchant.AgentId,
                merchant.AgentKey);
    }
}
