using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using TGS.GameProviders.Core.Avia;
using TGS.GameProviders.Core.Avia.ApplicationServices;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Core.Repositories.Models.Player;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Domains.SettingConfiguration.Avia;

namespace TGS.GameProviders.Core.UnitTests.Avia
{
    [TestFixture]
    public class AviaKickPlayerCommandServiceUnitTest
    {
        [Test]
        public async Task Normal_Process()
        {
            var faceLogger = NullLogger<AviaKickPlayerCommandService>.Instance;
            var fakeAviaApiClient = Substitute.For<IAviaApiClient>();
            var fakeAviaService = Substitute.For<IAviaService>();
            var fakePlayerRepository = Substitute.For<IPlayerRepository>();

            var fakeService = new AviaKickPlayerCommandService(
                faceLogger,
                fakeAviaApiClient,
                fakeAviaService,
                fakePlayerRepository);

            var command = new AviaKickPlayerCommand()
            {
                PlayerCode = "4ucmqplxb029"
            };

            var playerModel = new PlayerModel(
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

            var brandMerchant = new AviaBrandMerchant()
            {
                BrandCode = "Default",
                ApiUrl = new("https://api.aviaapi.vip"),
                Authorization = "81e2d1a24c684a76a710a70f4900a353",
                PlayerPassword = "TgsP@ssw0rd"
            };

            _ = fakeAviaService.GetBrandMerchantAsync(playerModel.BrandCode).Returns(ValueTask.FromResult(brandMerchant));
            _ = fakePlayerRepository.GetAsync(command.PlayerCode).Returns(Task.FromResult(playerModel));

            _ = fakeAviaApiClient.KickAsync(
                brandMerchant.ApiUrl,
                brandMerchant.Authorization,
                command.PlayerCode);

            await fakeService.ExecuteAsync(command).ConfigureAwait(false);
        }
    }
}
