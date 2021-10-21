using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.ApiClients;
using TGS.GameProviders.ApiClients.TCG;
using TGS.GameProviders.ApiClients.TCG.Models;
using TGS.GameProviders.Repositories;
using TGS.GameProviders.Repositories.Domains;
using TGS.ServiceUtility;

namespace TGS.GameProviders.Core.TCG.ApplicationServices
{
    public class TCGGameUrlGetService : IGetService<TCGGameUrlQuery, GameUrlResult>
    {
        private readonly ITCGApiClient _client;
        private readonly ITCGGamePlayerCreate _gamePlayerCreate;
        private readonly IGameRepository _gameRepository;
        private readonly ILogger<TCGGameUrlGetService> _logger;
        private readonly IReadOnlyDictionary<string, Func<Uri, string, string, string, bool, TCGLaunchGameRequest, ValueTask<TCGLaunchGameResponse>>> _methodMapper;
        private readonly ITCGService _service;

        public TCGGameUrlGetService(
            ITCGApiClient client,
            IGameRepository gameRepository,
            ILogger<TCGGameUrlGetService> logger,
            ITCGService service,
            ITCGGamePlayerCreate gamePlayerCreate
            )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _gamePlayerCreate = gamePlayerCreate ?? throw new ArgumentNullException(nameof(gamePlayerCreate));
            _methodMapper = new Dictionary<string, Func<Uri, string, string, string, bool, TCGLaunchGameRequest, ValueTask<TCGLaunchGameResponse>>>
            {
                { "384",SEAAsync },
                { "2",ELOTTOAsync },
                { "420",LOTTOVNAsync }
            };
        }

        public async ValueTask<GameUrlResult> GetAsync(TCGGameUrlQuery command, CancellationToken cancellationToken = default)
        {
            var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);
            var cultureCode = await _service.GetGameProviderCultureCodeAsync(command.Language).ConfigureAwait(false);
            var merchant = await _service.GetMerchantAsync(command.CurrencyCode).ConfigureAwait(false);
            if (merchant == null)
            {
                return null;
            }
            var getGame = await _gameRepository.GetGameAsync(command.GameId.Value).ConfigureAwait(false);

            if (getGame == null)
            {
                return null;
            }

            if (!_methodMapper.TryGetValue(getGame.ExternalId.ToUpper(), out var execute))
            {
                return null;
            }

            var data = new TCGLaunchGameRequest()
            {
                UserName = command.PlayerCode,
                Language = cultureCode
            };

            var response = await execute(
                configuration.ApiUrl,
                merchant.MerchantCode,
                merchant.DESKey,
                merchant.SHA256Key,
                command.PlatformType == PlatformType.Mobile,
                data
                ).ConfigureAwait(false);

            if (response.Status == 15)
            {
                if (await _gamePlayerCreate.CreatePlayerAsync(configuration.ApiUrl, merchant.MerchantCode, merchant.DESKey, merchant.SHA256Key, merchant.PlayerPassword, command.PlayerCode))
                {
                    response = await execute(
                        configuration.ApiUrl,
                        merchant.MerchantCode,
                        merchant.DESKey,
                        merchant.SHA256Key,
                        command.PlatformType == PlatformType.Mobile,
                        data).ConfigureAwait(false);
                }
            }
            return (response.Status == 0) ? new() { Url = new($"{ merchant.GameUrl}{response.GameUrl}") } : null;
        }

        private ValueTask<TCGLaunchGameResponse> ELOTTOAsync(Uri apiUrl, string merchantCode, string desKey, string sha256Key, bool IsMobile, TCGLaunchGameRequest source)
        {
            var data = new TCGLaunchGameByELOTTORequest()
            {
                UserName = source.UserName,
                Platform = "html5",
                GameMode = ((int)TCGPlayerGameMode.RealPlayer).ToString(),
                GameCode = "Lobby",
                Language = source.Language,
                LotteryBetMode = "Elott_Traditional",
                View = "Lobby"
            };
            if (IsMobile)
            {
                data.GameCode = "game_list";
                data.LotteryBetMode = "Elott_Traditional_Mobile_V2";
                data.View = "game_list";
            }
            return _client.LaunchGameAsync(
                apiUrl,
                merchantCode,
                desKey,
                sha256Key,
                data);
        }

        private ValueTask<TCGLaunchGameResponse> LOTTOVNAsync(Uri apiUrl, string merchantCode, string desKey, string sha256Key, bool IsMobile, TCGLaunchGameRequest source)
        {
            var data = new TCGLaunchGameByLOTTOVNRequest()
            {
                UserName = source.UserName,
                Platform = "WEB",
                GameMode = ((int)TCGPlayerGameMode.RealPlayer).ToString(),
                GameCode = "lobby",
                Language = source.Language,
                LotteryBetMode = "Elott",
                View = "lobby"
            };

            return _client.LaunchGameAsync(
                apiUrl,
                merchantCode,
                desKey,
                sha256Key,
                data);
        }

        private ValueTask<TCGLaunchGameResponse> SEAAsync(Uri apiUrl, string merchantCode, string desKey, string sha256Key, bool IsMobile, TCGLaunchGameRequest source)
        {
            var data = new TCGLaunchGameBySEARequest()
            {
                UserName = source.UserName,
                Platform = "html5",
                GameMode = ((int)TCGPlayerGameMode.RealPlayer).ToString(),
                GameCode = "lobby",
                Language = source.Language,
                LotteryBetMode = "SEA_Tradition",
                View = "lobby"
            };
            return _client.LaunchGameAsync(
                apiUrl,
                merchantCode,
                desKey,
                sha256Key,
                data);
        }
    }
}
