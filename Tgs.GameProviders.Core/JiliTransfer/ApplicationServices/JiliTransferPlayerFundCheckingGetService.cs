using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.JiliService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.JiliTransfer.ApplicationServices;

public class JiliTransferPlayerFundCheckingGetService : IGetService<JiliTransferPlayerFundCheckingQuery, PlayerFundCheckingResult>
{
    private readonly IBrandRepository _brand;
    private readonly IGameProviderRepository _gameProvider;
    private readonly IGameInterfaceJiliServiceGrpc _grpc;
    private readonly IPlayerRepository _player;

    public JiliTransferPlayerFundCheckingGetService(
        IBrandRepository brand,
        IGameProviderRepository gameProvider,
        IGameInterfaceJiliServiceGrpc grpc,
        IPlayerRepository player)
    {
        _brand = brand ?? throw new ArgumentNullException(nameof(brand));
        _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
        _grpc = grpc ?? throw new ArgumentNullException(nameof(grpc));
        _player = player ?? throw new ArgumentNullException(nameof(player));
    }

    public async ValueTask<PlayerFundCheckingResult> GetAsync(JiliTransferPlayerFundCheckingQuery command, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _gameProvider.GetGameProviderIdAsync(GameProviderType.JiliTransfer).ConfigureAwait(false);
        var player = await _player.GetAsync(command.PlayerCode).ConfigureAwait(false);
        var gameProviderConfigurationId = await _brand.GetBrandGameProviderConfigurationIdAsync(gameProviderId, player.BrandId).ConfigureAwait(false);
        var configurationData = await _gameProvider.GetJiliTransferConfiguration(gameProviderConfigurationId).ConfigureAwait(false);

        var response = await _grpc.FundOrderCheckingAsync(
            apiUrl: configurationData.ApiUrl,
            agentId: configurationData.AgentId,
            agentKey: configurationData.AgentKey,
            transactionId: command.OrderId,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        return new() { OrderStatus = response };
    }
}
