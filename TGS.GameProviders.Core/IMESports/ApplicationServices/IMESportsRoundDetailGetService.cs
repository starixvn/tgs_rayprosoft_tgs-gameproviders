using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core.IMESports.Models;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Core.Repositories.Models.Brand;
using TGS.GameProviders.Core.Repositories.Models.Rounds;
using TGS.GameProviders.Domains.RoundDetail;
using TGS.GameProviders.Domains.SettingConfiguration;

namespace TGS.GameProviders.Core.IMESports.ApplicationServices
{
    internal class IMESportsRoundDetailGetService : IGetService<IMESportsRoundDetailQuery, RoundDetailResult>
    {
        private static readonly string _TimeFormat = "yyyy-MM-dd HH.mm.ss";

        private readonly ILogger<IMESportsRoundDetailGetService> _logger;
        private readonly IRoundRepository _roundRepository;
        private readonly IIMESportsService _service;
        private readonly IIMESportsMerchantMapper _merchantMapper;
        private readonly IIMESportsApiClient _client;
        private readonly JsonSerializerOptions _jsonSerialzierOptions;

        public IMESportsRoundDetailGetService(
            ILogger<IMESportsRoundDetailGetService> logger,
            IRoundRepository roundRepository,
            IIMESportsService service,
            IIMESportsMerchantMapper merchantMapper,
            IIMESportsApiClient client)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _roundRepository = roundRepository ?? throw new ArgumentNullException(nameof(roundRepository));
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
            var roundModel = await _roundRepository.GetAsync(query.RoundId).ConfigureAwait(false);
            if (roundModel == null)
            {
                throw new Exception($"This game round number could not be found.{query.RoundId}");
            }

            var gameProviderModel = await _service.GetGameProviderAsync(roundModel.GameProviderId).ConfigureAwait(false);
            if (gameProviderModel == null)
            {
                throw new Exception($"This game manufacturer does not exist.{roundModel.GameProviderId}");
            }

            var gameModel = await _service.GetGameAsync(roundModel.GameId).ConfigureAwait(false);
            if (gameModel == null)
            {
                throw new Exception($"This game Id could not be found.{roundModel.GameId}");
            }

            var brandModel = await _service.GetBrandAsync(roundModel.BrandId).ConfigureAwait(false);
            if (brandModel == null)
            {
                throw new Exception($"This brand Id could not be found.{roundModel.BrandId}");
            }

            var configuration = await _service.GetSettingConfigurationAsync().ConfigureAwait(false);

            var result = await GetRoundResultAsync(configuration, roundModel, brandModel, query.Language).ConfigureAwait(false);

            var value = result == null ?
                null :
                JsonSerializer.Serialize(result.Details, _jsonSerialzierOptions);

            return new RoundDetailResult
            {
                GameProviderName = gameProviderModel.Name,
                GameName = gameModel.Name,
                ExternalRoundId = roundModel.ExternalRoundId,
                BetAmount = roundModel.BetAmount,
                Turnover = roundModel.Turnover,
                Winloss = roundModel.WinLoss,
                GameResult = roundModel.ClientStatus.ToString(),
                Data = new RoundDetailDataBase[]
                {
                   new RoundDetailDataJson
                   {
                       Value = value
                   }
                }
            };
        }

        private async ValueTask<IMESportsHistoryResult> GetRoundResultAsync(
            IMESportsConfiguration configuration,
            RoundModel roundModel,
            BrandModel brandModel,
            string cultureCode)
        {
            var merchant = _merchantMapper.GetBrandMerchant(configuration.BrandMerchants, brandModel.Code);

            var gameProviderCultureCode = await _service
                .GetGameProviderLanguageCodeAsync(roundModel.GameProviderId, cultureCode)
                .ConfigureAwait(false);

            var createdOn = new DateTimeOffset(roundModel.ExternalCreatedOn!.Value).ToOffset(TimeSpan.FromHours(8));

            var startDate = createdOn.AddMinutes(-1).ToString(_TimeFormat);
            var endDate = createdOn.AddMinutes(1).ToString(_TimeFormat);

            var transactionId = roundModel.ExternalRoundId;
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

        private IMESportsHistoryResult GetHistoryResult(IEnumerable<IMESportsHistoryResult> results, string transactionId)
            => results.FirstOrDefault(r => r.BetId == transactionId);
    }
}