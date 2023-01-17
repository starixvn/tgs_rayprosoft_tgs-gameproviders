using Tgs.GameCommand.Client;
using Tgs.GameProviders.Core;

namespace Tgs.GameProviders.Repositories;

internal class BrandRepository : IBrandRepository
{
    private readonly IGameCommandClient _client;

    public BrandRepository(
        IGameCommandClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask<Guid> GetBrandGameProviderConfigurationIdAsync(
        Guid gameProviderId,
        Guid brandId,
        CancellationToken cancellationToken = default)
        => await _client
        .BrandGetGameProviderConfigurationIdAsync(
            brandId: brandId,
            gameProviderId: gameProviderId,
            cancellationToken: cancellationToken)
        .ConfigureAwait(false)
        ?? throw new Exception($"The gameprovider configuration id does not exist.(gameProviderId={gameProviderId}、brandId{brandId})");

    public async ValueTask<Guid> GetBrandIdAsync(
        string brandCode,
        CancellationToken cancellationToken = default)
        => await _client
        .BrandGetBrandIdAsync(
            brandCode,
            cancellationToken)
        .ConfigureAwait(false)
        ?? throw new Exception($"The brand id does not exist.({brandCode})");
}
