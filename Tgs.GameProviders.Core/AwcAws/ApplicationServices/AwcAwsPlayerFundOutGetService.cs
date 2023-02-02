using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.AwcService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.AwcAws.ApplicationServices;

public class AwcAwsPlayerFundOutGetService : IGetService<AwcAwsPlayerFundOutQuery, PlayerFundResult>
{
    private readonly IBrandRepository _brand;
    private readonly IGameInterfaceAwcServiceClient _client;
    private readonly IGameProviderRepository _gameProvider;
    private readonly IPlayerRepository _player;
    private readonly ISettingRepository _setting;

    public AwcAwsPlayerFundOutGetService(
        IBrandRepository brand,
        IGameProviderRepository gameProvider,
        IGameInterfaceAwcServiceClient client,
        IPlayerRepository player,
        ISettingRepository setting)
    {
        _brand = brand ?? throw new ArgumentNullException(nameof(brand));
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _player = player ?? throw new ArgumentNullException(nameof(player));
        _setting = setting ?? throw new ArgumentNullException(nameof(setting));
    }

    public async ValueTask<PlayerFundResult> GetAsync(AwcAwsPlayerFundOutQuery command, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProvider.GetGameProviderIdAsync(GameProviderType.AWC_AWS).ConfigureAwait(false);
        var configuration = await _setting.GetAwcAwsConfigurationAsync().ConfigureAwait(false);
        var player = await _player.GetAsync(command.PlayerCode).ConfigureAwait(false);
        var gameProviderConfigurationId = await _brand.GetBrandGameProviderConfigurationIdAsync(gameProviderId, player.BrandId).ConfigureAwait(false);
        var configurationData = await _gameProvider.GetAwcConfigurationData(gameProviderConfigurationId).ConfigureAwait(false);

        var reply = await _client.FundOutAsync(
            apiUrl: configuration.ApiUrl,
            agentId: configurationData.AgentId,
            cert: configurationData.Cert,
            playerCode: player.PlayerCode,
            transactionId: command.OrderId,
            amount: command.Amount,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        return (reply.Success) ? new() { OrderId = reply.GameProviderTransactionId } : null;
    }
}
