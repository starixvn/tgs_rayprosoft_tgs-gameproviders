using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.ApiClients.SBO.Models;
using TGS.GameProviders.Domains;
using Tgs.Logging;

namespace TGS.GameProviders.ApiClients.SBO
{
    public class SBOApiClient : ISBOApiClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<SBOApiClient> _logger;
        private readonly ITgsLog _tgsLog;

        public SBOApiClient(
            HttpClient client,
            ILogger<SBOApiClient> logger,
            ITgsLog tgsLog)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tgsLog = tgsLog ?? throw new ArgumentNullException(nameof(tgsLog));
        }


        ValueTask<LoginResponse> ISBOApiClient.LoginAsync(Uri apiUrl, LoginRequest request)
        {
            var prefix = $"{GameProviderType.SBOBET}.Login";
            return ExecuteJsonRequestAsync<LoginRequest, LoginResponse>(HttpMethod.Post, apiUrl,
                "/web-root/restricted/player/login.aspx", request, logPrefix: prefix);
        }

        ValueTask<LogoutResponse> ISBOApiClient.LogoutAsync(Uri apiUrl, LogoutRequest request)
        {
            var prefix = $"{Consts.GameProvider.SBOBET}.Logout";
            return ExecuteJsonRequestAsync<LogoutRequest, LogoutResponse>(HttpMethod.Post, apiUrl,
                "/web-root/restricted/player/logout.aspx", request, logPrefix: prefix);
        }

        ValueTask<RegisterPlayerResponse> ISBOApiClient.RegisterPlayerAsync(Uri apiUrl, RegisterPlayerRequest request)
        {
            var prefix = $"{Consts.GameProvider.SBOBET}.RegisterPlayer";
            return ExecuteJsonRequestAsync<RegisterPlayerRequest, RegisterPlayerResponse>(
                HttpMethod.Post, apiUrl,
                "/web-root/restricted/player/register-player.aspx",
                request, logPrefix: prefix);
        }

        ValueTask<GetRoundHistoryResponse> ISBOApiClient.GetRoundHistoryAsync(Uri apiUrl,
            GetRoundHistoryRequest request)
        {
            var options = new JsonSerializerOptions
            {
                Converters =
                {
                    new NumberToStringConverter<>()
                }
            };

            var prefix = $"{Consts.GameProvider.SBOBET}.RoundHistory";
            return ExecuteJsonRequestAsync<GetRoundHistoryRequest, GetRoundHistoryResponse>(
                HttpMethod.Post, apiUrl, "/web-root/restricted/report/get-bet-list-by-refnos.aspx",
                request, logPrefix: prefix, options: options);
        }

        ValueTask<GetBetHistoryResponse> ISBOApiClient.GetBetHistoryAsync(Uri apiUrl, GetBetHistoryRequest request)
        {
            var options = new JsonSerializerOptions
            {
                Converters =
                {
                    new NumberToStringConverter()
                }
            };

            var prefix = $"{Consts.GameProvider.SBOBET}.BetHistory";
            return ExecuteJsonRequestAsync<GetBetHistoryRequest, GetBetHistoryResponse>(
                HttpMethod.Post, apiUrl, "/web-root/restricted/report/v2/get-bet-list-by-modify-date.aspx",
                request, logPrefix: prefix, options: options);
        }

        ValueTask<UpdatePlayerBetLimitsResponse> ISBOApiClient.UpdatePlayerBetLimitsAsync(Uri apiUrl,
            UpdatePlayerBetLimitsRequest request)
        {
            var prefix = $"{Consts.GameProvider.SBOBET}.UpdatePlayerBetLimit";
            return ExecuteJsonRequestAsync<UpdatePlayerBetLimitsRequest, UpdatePlayerBetLimitsResponse>(
                HttpMethod.Post, apiUrl,
                "/web-root/restricted/player/update-player-bet-setting-by-sportid-and-markettype.aspx",
                request, logPrefix: prefix);
        }
    }
}