using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.Jili.ApplicationServices;

internal class JiliRoundDetailGetService : IGetService<JiliRoundDetailQuery, RoundDetailResult>
{
    private readonly IJiliApiClient _client;
    private readonly IJiliMerchantMapper _merchantMapper;
    private readonly IPlayerRepository _playerService;
    private readonly IRoundRepository _roundService;
    private readonly IJiliService _service;

    public JiliRoundDetailGetService(
        IRoundRepository roundService,
        IPlayerRepository playerService,
        IJiliService service,
        IJiliMerchantMapper merchantMapper,
        IJiliApiClient client)
    {
        _roundService = roundService ?? throw new ArgumentNullException(nameof(roundService));
        _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _merchantMapper = merchantMapper ?? throw new ArgumentNullException(nameof(merchantMapper));
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask<RoundDetailResult> GetAsync(
        JiliRoundDetailQuery query,
        CancellationToken cancellationToken = default)
    {
        var roundInfo = await _roundService
            .GetAsync(
            roundId: query.RoundId,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var playerModel = await _playerService.GetAsync(roundInfo.PlayerCode).ConfigureAwait(false);
        var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);
        var merchant = _merchantMapper.GetMerchant(configuration.Merchants, playerModel.BrandCode);

        var gameProviderCultureCode = await _service
            .GetGameProviderLanguageCodeAsync(roundInfo.GameProviderId, query.Language)
            .ConfigureAwait(false);

        var response = await _client
            .GetGameDetailUrlAsync(configuration.ApiUrl,
                merchant.AgentId,
                merchant.AgentKey,
                gameProviderCultureCode,
                roundInfo.ExternalRoundId)
            .ConfigureAwait(false);

        var url = response.ErrorCode == 0 ? response.Data.Url : string.Empty;

        return new RoundDetailResult
        {
            Data = new RoundDetailDataBase[]
            {
                new RoundDetailDataUrl
                {
                    Url = url
                }
            }
        };
    }
}
