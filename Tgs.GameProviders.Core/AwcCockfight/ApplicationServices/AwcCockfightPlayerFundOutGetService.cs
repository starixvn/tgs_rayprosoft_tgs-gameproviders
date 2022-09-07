using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.AwcService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.AwcCockfight.ApplicationServices;

public class AwcCockfightPlayerFundOutGetService : IGetService<AwcCockfightPlayerFundOutQuery, PlayerFundResult>
{
    private readonly IBrandRepository _brand;
    private readonly IGameProviderRepository _gameProvider;
    private readonly IGameInterfaceAwcServiceGrpc _grpc;
    private readonly IPlayerRepository _player;
    private readonly ISettingRepository _setting;

    public AwcCockfightPlayerFundOutGetService(
        IBrandRepository brand,
        IGameProviderRepository gameProvider,
        IGameInterfaceAwcServiceGrpc grpc,
        IPlayerRepository player,
        ISettingRepository setting)
    {
        _brand = brand ?? throw new ArgumentNullException(nameof(brand));
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _grpc = grpc ?? throw new ArgumentNullException(nameof(grpc));
        _player = player ?? throw new ArgumentNullException(nameof(player));
        _setting = setting ?? throw new ArgumentNullException(nameof(setting));
    }

    public async ValueTask<PlayerFundResult> GetAsync(AwcCockfightPlayerFundOutQuery command, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProvider.GetGameProviderIdAsync(GameProviderType.AWC_COCKFIGHT).ConfigureAwait(false);
        var configuration = await _setting.GetAwcCockfightConfigurationAsync().ConfigureAwait(false);
        var player = await _player.GetAsync(command.PlayerCode).ConfigureAwait(false);
        var gameProviderConfigurationId = await _brand.GetBrandGameProviderConfigurationIdAsync(gameProviderId, player.BrandId).ConfigureAwait(false);
        var configurationData = await _gameProvider.GetAwcConfigurationData(gameProviderConfigurationId).ConfigureAwait(false);

        var reply = await _grpc.FundOutAsync(
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
