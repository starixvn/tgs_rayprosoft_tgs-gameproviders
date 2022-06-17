using NSubstitute;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Services.AsiaGaming;

namespace Tgs.GameProviders.Services.UnitTest.AsiaGaming;

internal class AsiaGamingServiceTests
{
    [Test]
    public async Task Can_Get_Desktop_Game_Domain()
    {
        var fakeBrandService = Substitute.For<IBrandService>();
        var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
        var fakeGameRepository = Substitute.For<IGameRepository>();
        var fakeSettingRepository = Substitute.For<ISettingRepository>();

        var sut = new AsiaGamingService(
            fakeSettingRepository,
            fakeGameProviderRepository,
            fakeGameRepository,
            fakeBrandService);

        var brandId = Guid.NewGuid();
        var gameProviderId = Guid.NewGuid();
        var platformType = PlatformType.Desktop;
        var gameProviderConfigurationId = Guid.NewGuid();

        var endpoints = (DesktopEndpoint: "http://desktop.google.com", MobileEndpoint: "http://mobile.google.com");

        _ = fakeBrandService.GetBrandGameProviderConfigurationIdAsync(gameProviderId, brandId)
            .Returns(gameProviderConfigurationId);

        _ = fakeGameProviderRepository.GetGameEndpointsAsync(gameProviderConfigurationId)
            .Returns(endpoints);

        var actual = await sut.GetGameDomainAsync(brandId, gameProviderId, platformType).ConfigureAwait(false);

        Assert.AreEqual(endpoints.DesktopEndpoint, actual);
    }

    [Test]
    public async Task Can_Get_Mobile_Game_Domain()
    {
        var fakeBrandService = Substitute.For<IBrandService>();
        var fakeGameProviderRepository = Substitute.For<IGameProviderRepository>();
        var fakeGameRepository = Substitute.For<IGameRepository>();
        var fakeSettingRepository = Substitute.For<ISettingRepository>();

        var sut = new AsiaGamingService(
            fakeSettingRepository,
            fakeGameProviderRepository,
            fakeGameRepository,
            fakeBrandService);

        var brandId = Guid.NewGuid();
        var gameProviderId = Guid.NewGuid();
        var platformType = PlatformType.Mobile;
        var gameProviderConfigurationId = Guid.NewGuid();

        var endpoints = (DesktopEndpoint: "http://desktop.google.com", MobileEndpoint: "http://mobile.google.com");

        _ = fakeBrandService.GetBrandGameProviderConfigurationIdAsync(gameProviderId, brandId)
            .Returns(gameProviderConfigurationId);

        _ = fakeGameProviderRepository.GetGameEndpointsAsync(gameProviderConfigurationId)
            .Returns(endpoints);

        var actual = await sut.GetGameDomainAsync(brandId, gameProviderId, platformType).ConfigureAwait(false);

        Assert.AreEqual(endpoints.MobileEndpoint, actual);
    }
}