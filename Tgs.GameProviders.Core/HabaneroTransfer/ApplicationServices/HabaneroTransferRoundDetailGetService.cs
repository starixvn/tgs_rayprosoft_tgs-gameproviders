using System;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Tgs.GameProviders.Domains;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.HabaneroTransfer.ApplicationServices;

internal class HabaneroTransferRoundDetailGetService : IGetService<HabaneroTransferRoundDetailQuery, RoundDetailResult>
{
    private readonly IBrandRepository _brand;
    private readonly IGameProviderRepository _gameProvider;
    private readonly IPlayerRepository _player;
    private readonly IRoundRepository _roundService;

    public HabaneroTransferRoundDetailGetService(
        IBrandRepository brand,
        IGameProviderRepository gameProvider,
        IPlayerRepository player,
        IRoundRepository roundService)
    {
        _brand = brand ?? throw new ArgumentNullException(nameof(brand));
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _player = player ?? throw new ArgumentNullException(nameof(player));
        _roundService = roundService ?? throw new ArgumentNullException(nameof(roundService));
    }

    public async ValueTask<RoundDetailResult> GetAsync(HabaneroTransferRoundDetailQuery query, CancellationToken cancellationToken = default)
    {
        var roundInfo = await _roundService
            .GetAsync(
            roundId: query.RoundId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var gameProviderId = await _gameProvider.GetGameProviderIdAsync(GameProviderType.HabaneroTransfer).ConfigureAwait(false);
        var player = await _player.GetAsync(roundInfo.PlayerCode).ConfigureAwait(false);
        var gameProviderConfigurationId = await _brand.GetBrandGameProviderConfigurationIdAsync(gameProviderId, player.BrandId).ConfigureAwait(false);
        var configuration = await _gameProvider.GetHabaneroTransferConfigurationAsync(gameProviderConfigurationId).ConfigureAwait(false);
        var cultureCode = await _gameProvider.GetGameProviderCultureCodeAsync(gameProviderId, query.Language).ConfigureAwait(false) ?? "en";

        var hash = Sha256Hash($"{roundInfo.ExternalRoundId.ToLower()}{configuration.BrandId.ToLower()}{configuration.ApiKey.ToLower()}");

        var queryStrings = HttpUtility.ParseQueryString(string.Empty);

        queryStrings.Add("brandId", configuration.BrandId);
        queryStrings.Add("FriendlyId", roundInfo.ExternalRoundId);
        queryStrings.Add("hash", hash);
        queryStrings.Add("viewtype", "game");
        queryStrings.Add("locale", cultureCode);

        var url = new Uri(configuration.GameApiUrl, $"/games/history?{queryStrings}");

        return new()
        {
            Data = new RoundDetailDataBase[]
            {
                new RoundDetailDataUrl { Url = url.ToString() }
}
        };
    }

    private static string Sha256Hash(string value)
        => new BigInteger(SHA256.HashData(Encoding.UTF8.GetBytes(value)), isBigEndian: true)
            .ToString("x");
}
