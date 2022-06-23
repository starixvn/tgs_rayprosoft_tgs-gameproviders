using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.GameProviders.Core.IMESports.Models;
using Tgs.GameProviders.Domains.RoundDetail;
using Tgs.GameProviders.Domains.SettingConfiguration;
using Tgs.GameProviders.QueryServer.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.IMESports.ApplicationServices
{
    internal class IMESportsRoundDetailGetService : IGetService<IMESportsRoundDetailQuery, RoundDetailResult>
    {
        private static readonly string _TimeFormat = "yyyy-MM-dd HH.mm.ss";

        private readonly IIMESportsApiClient _client;
        private readonly JsonSerializerOptions _jsonSerialzierOptions;
        private readonly ILogger<IMESportsRoundDetailGetService> _logger;
        private readonly IIMESportsMerchantMapper _merchantMapper;
        private readonly IRoundService _roundService;
        private readonly IIMESportsService _service;

        public IMESportsRoundDetailGetService(
            ILogger<IMESportsRoundDetailGetService> logger,
            IRoundService roundService,
            IIMESportsService service,
            IIMESportsMerchantMapper merchantMapper,
            IIMESportsApiClient client)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _roundService = roundService ?? throw new ArgumentNullException(nameof(roundService));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _merchantMapper = merchantMapper ?? throw new ArgumentNullException(nameof(merchantMapper));
            _client = client ?? throw new ArgumentNullException(nameof(client));

            _jsonSerialzierOptions = new JsonSerializerOptions
            {
                WriteIndented = false
            };
        }

        public async ValueTask<RoundDetailResult> GetAsync(
            IMESportsRoundDetailQuery query,
            CancellationToken cancellationToken = default)
        {
            var roundInfo = await _roundService.GetAsync(query.RoundId).ConfigureAwait(false);
            if (roundInfo == null)
            {
                throw new Exception($"This game round number could not be found.{query.RoundId}");
            }

            var playerModel = await _service.GetPlayerAsync(roundInfo.PlayerCode).ConfigureAwait(false);
            var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);

            var result = await GetRoundResultAsync(configuration, roundInfo, playerModel, query.Language).ConfigureAwait(false);

            var value = result == null ?
                null :
                JsonSerializer.Serialize(result.Details, _jsonSerialzierOptions);

            return new RoundDetailResult
            {
                Data = new RoundDetailDataBase[]
                {
                   new RoundDetailDataJson
                   {
                       Value = value
                   }
                }
            };
        }

        private IMESportsHistoryResult GetHistoryResult(IEnumerable<IMESportsHistoryResult> results, string transactionId)
            => results.FirstOrDefault(r => r.BetId == transactionId);

        private async ValueTask<IMESportsHistoryResult> GetRoundResultAsync(
            IMESportsConfiguration configuration,
            RoundInfo roundInfo,
            PlayerModel playerInfo,
            string cultureCode)
        {
            var merchant = _merchantMapper.GetBrandMerchant(configuration.BrandMerchants, playerInfo.BrandCode);

            var gameProviderCultureCode = await _service
                .GetGameProviderLanguageCodeAsync(roundInfo.GameProviderId, cultureCode)
                .ConfigureAwait(false);

            var createdOn = new DateTimeOffset(roundInfo.ExternalCreatedOn!.Value.UtcDateTime).ToOffset(TimeSpan.FromHours(8));

            var startDate = createdOn.AddMinutes(-1).ToString(_TimeFormat);
            var endDate = createdOn.AddMinutes(1).ToString(_TimeFormat);

            var transactionId = roundInfo.ExternalRoundId;
            var currentPage = 1;

            int totalPage;
            IMESportsHistoryResult result;

            do
            {
                var response = await _client.GetRoundHistoryAsync(
                       merchant.ApiUrl,
                       new()
                       {
                           MerchantCode = merchant.MerchantCode,
                           ProductWallet = configuration.ProductWallet,
                           StartDate = startDate,
                           EndDate = endDate,
                           Page = currentPage,
                           Language = gameProviderCultureCode
                       }).ConfigureAwait(false);

                if (response.ErrorCode != IMESportsGameApiError.Success)
                {
                    return null;
                }

                totalPage = response.Pagination.TotalPage;
                result = GetHistoryResult(response.Results, transactionId);
                currentPage++;
            } while (result is null && currentPage <= totalPage);

            return result;
        }
    }
}
