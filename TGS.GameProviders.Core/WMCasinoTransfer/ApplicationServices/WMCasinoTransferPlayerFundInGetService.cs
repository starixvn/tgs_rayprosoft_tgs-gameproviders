using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.WMCasinoService.Client;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.WMCasinoTransfer.ApplicationServices
{
    public class WMCasinoTransferPlayerFundInGetService : IGetService<WMCasinoTransferPlayerFundInQuery, PlayerFundResult>
    {
        private readonly IBrandService _brand;
        private readonly IGameProviderRepository _gameProvider;
        private readonly IGameInterfaceWMCasinoServiceGrpc _grpc;
        private readonly IPlayerRepository _player;
        private readonly ISettingRepository _setting;

        public WMCasinoTransferPlayerFundInGetService(
            IBrandService brand,
            IGameProviderRepository gameProvider,
            IGameInterfaceWMCasinoServiceGrpc grpc,
            IPlayerRepository player,
            ISettingRepository setting)
        {
            _brand = brand ?? throw new ArgumentNullException(nameof(brand));
            _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
            _grpc = grpc ?? throw new ArgumentNullException(nameof(grpc));
            _player = player ?? throw new ArgumentNullException(nameof(player));
            _setting = setting ?? throw new ArgumentNullException(nameof(setting));
        }

        public async ValueTask<PlayerFundResult> GetAsync(WMCasinoTransferPlayerFundInQuery command, CancellationToken cancellationToken = default)
        {
            var gameProviderId = await _gameProvider.GetGameProviderIdAsync(GameProviderType.WMCasinoTransfer).ConfigureAwait(false);
            var configuration = await _setting.GetWMCasinoTransferConfigurationAsync().ConfigureAwait(false);
            var player = await _player.GetAsync(command.PlayerCode).ConfigureAwait(false);
            var gameProviderConfigurationId = await _brand.GetBrandGameProviderConfigurationIdAsync(gameProviderId, player.BrandId).ConfigureAwait(false);
            var configurationData = await _gameProvider.GetWMCasinoTransferConfigurationData(gameProviderConfigurationId).ConfigureAwait(false);

            var user = $"wmtf_{player.PlayerCode}_{player.BrandCode}";

            var reply = await _grpc.FundInAsync(
                apiUrl: configuration.ApiUrl,
                vendorId: configurationData.VendorId,
                signature: configurationData.Signature,
                user: user,
                transactionId: command.OrderId,
                amount: command.Amount,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            return (reply.Success) ? new() { OrderId = reply.GameProviderTransactionId } : null;
        }
    }
}