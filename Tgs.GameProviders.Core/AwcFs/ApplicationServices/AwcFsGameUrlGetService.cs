using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.GameInterface.AwcService.Client;
using Tgs.GameInterface.AwcService.Protos;
using Tgs.GameProviders.Domains;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.AwcFs.ApplicationServices
{
    public class AwcFsGameUrlGetService : IGetService<AwcFsGameUrlQuery, GameUrlResult>
    {
        private readonly IBrandService _brand;
        private readonly IGameRepository _game;
        private readonly IAwcFsGamePlayer _gamePlayer;
        private readonly IGameProviderRepository _gameProvider;
        private readonly IGameInterfaceAwcServiceGrpc _grpc;
        private readonly IPlayerRepository _player;
        private readonly ISettingRepository _setting;

        public AwcFsGameUrlGetService(
            IBrandService brand,
            IGameRepository game,
            IAwcFsGamePlayer gamePlayer,
            IGameProviderRepository gameProvider,
            IGameInterfaceAwcServiceGrpc grpc,
            IPlayerRepository player,
            ISettingRepository setting)
        {
            _brand = brand ?? throw new ArgumentNullException(nameof(brand));
            _game = game ?? throw new ArgumentNullException(nameof(game));
            _gamePlayer = gamePlayer ?? throw new ArgumentNullException(nameof(gamePlayer));
            _gameProvider = gameProvider ?? throw new ArgumentNullException(nameof(gameProvider));
            _grpc = grpc ?? throw new ArgumentNullException(nameof(grpc));
            _player = player ?? throw new ArgumentNullException(nameof(player));
            _setting = setting ?? throw new ArgumentNullException(nameof(setting));
        }

        public async ValueTask<GameUrlResult> GetAsync(AwcFsGameUrlQuery command, CancellationToken cancellationToken = default)
        {
            var gameProviderId = await _gameProvider.GetGameProviderIdAsync(GameProviderType.AWC_FS).ConfigureAwait(false);
            var configuration = await _setting.GetAwcFsConfigurationAsync().ConfigureAwait(false);
            var player = await _player.GetAsync(command.PlayerCode).ConfigureAwait(false);
            var gameProviderConfigurationId = await _brand.GetBrandGameProviderConfigurationIdAsync(gameProviderId, player.BrandId).ConfigureAwait(false);
            var configurationData = await _gameProvider.GetAwcConfigurationData(gameProviderConfigurationId).ConfigureAwait(false);
            var cultureCode = await _gameProvider.GetGameProviderCultureCodeAsync(gameProviderId, command.Language) ?? "en";
            var game = await _game.GetAsync(command.GameId.Value).ConfigureAwait(false)
                 ?? throw new Exception($"The game for this game product does not exist.({command.GameId})");

            var request = new GameInterfaceAwcServiceGameUrlRequest()
            {
                ApiUrl = configuration.ApiUrl.ToString(),
                AgentId = configurationData.AgentId,
                Cert = configurationData.Cert,
                PlayerCode = command.PlayerCode,
                BackUrl = command.BackUrl.ToString(),
                IsMobileLogin = command.PlatformType == PlatformType.Mobile,
                ExternalGameId = game.GameExternalId,
                Language = cultureCode
            };

            var response = await _grpc.GameUrlByAwcFsAsync(
                request: request,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            if (response.Success && !response.IsPlayerExist)
            {
                await _gamePlayer.CreatePlayerAsync(
                      configuration,
                      configurationData,
                      player.PlayerCode,
                      player.DefaultWalletCurrencyCode,
                      command.Language,
                      cancellationToken).ConfigureAwait(false);

                response = await _grpc.GameUrlByAwcFsAsync(
                    request: request,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
            }

            return (response != null) ? new() { Url = new(response.GameUrl) } : null;
        }
    }
}
