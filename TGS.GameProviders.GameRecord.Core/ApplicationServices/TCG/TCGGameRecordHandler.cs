using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TGS.GameProviders.Core;
using TGS.GameProviders.TCG.FTPApi.Client;
using TGS.ServiceUtility;

namespace TGS.GameProviders.GameRecord.Core.ApplicationServices.TCG
{
    public class TCGGameRecordHandler : IGetService<TCGGameRecordHandlerQuery, TCGGameRecordHandlerResult>
    {
        private readonly ITCGFtpServiceClient _ftpServiceClient;
        private readonly IGameRecordProcessor _gameRoundsProcessor;
        private readonly IReadOnlyDictionary<string, string> _productTypeMappers;
        private readonly ITCGService _service;
        private readonly IReadOnlyDictionary<string, Func<string, string, string, ValueTask<GetSettledFileContentResponse>>> _settledFileContentMappers;
        private readonly IReadOnlyDictionary<string, Func<string, string, ValueTask<string[]>>> _settledFileListMappers;

        public TCGGameRecordHandler(
            ITCGService service,
            IGameRecordProcessor gameRoundsProcessor,
            ITCGFtpServiceClient ftpServiceClient
            )
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _gameRoundsProcessor = gameRoundsProcessor ?? throw new ArgumentNullException(nameof(gameRoundsProcessor));
            _ftpServiceClient = ftpServiceClient ?? throw new ArgumentNullException(nameof(ftpServiceClient));
            _settledFileListMappers = new Dictionary<string, Func<string, string, ValueTask<string[]>>>() {
            {"TCG_LOTTO",_ftpServiceClient.GetTcgELottoSettledFileListAsync},
            {"TCG_LOTTO_VN",_ftpServiceClient.GetTcgLottoVNSettledFileListAsync},
            {"TCG_SEA",_ftpServiceClient.GetTcgSEASettledFileListAsync}
            };
            _settledFileContentMappers = new Dictionary<string, Func<string, string, string, ValueTask<GetSettledFileContentResponse>>>() {
            {"TCG_LOTTO",_ftpServiceClient.GetTcgELottoSettledFileContentAsync},
            {"TCG_LOTTO_VN",_ftpServiceClient.GetTcgLottoVNSettledFileContentAsync},
            {"TCG_SEA",_ftpServiceClient.GetTcgSEASettledFileContentAsync}
            };
            _productTypeMappers = new Dictionary<string, string>() {
            {"TCG_LOTTO","2"},
            {"TCG_LOTTO_VN","420"},
            {"TCG_SEA","384"}
        };
        }

        public async ValueTask<TCGGameRecordHandlerResult> GetAsync(TCGGameRecordHandlerQuery query, CancellationToken cancellationToken = default)
        {
            if (!_productTypeMappers.TryGetValue(query.SubGP, out string externalGameId))
            {
                return new()
                {
                    Success = false,
                };
            }
            _settledFileListMappers.TryGetValue(query.SubGP, out var fileListExecute);

            var gameProviderId = await _service.GetGameProviderIdAsync().ConfigureAwait(false);

            var nowDay = DateTimeOffset.Parse(DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(8)).ToString("yyyy-MM-dd"));
            var lastGameRecorDirectory = (string.IsNullOrEmpty(query.LastGameRecorDirectory)) ? nowDay.ToString("yyyyMMdd") : query.LastGameRecorDirectory;
            var lastGameRecorFile = query.LastGameRecorFile;
            if (query.IsDesignation)
            {
                string[] getFileList;
                if (string.IsNullOrEmpty(lastGameRecorFile))
                {
                    getFileList = await fileListExecute(query.MerchantCode, lastGameRecorDirectory).ConfigureAwait(false);
                    if (getFileList == null)
                    {
                        return new()
                        {
                            Success = false,
                        };
                    }
                }
                else
                {
                    getFileList = new[] { lastGameRecorFile };
                }

                lastGameRecorFile = await ProcessFileListAsync(gameProviderId, externalGameId, query.SubGP, query.MerchantCode, lastGameRecorDirectory, getFileList).ConfigureAwait(false);
            }
            else
            {
                while (true)
                {
                    var getFileList = await fileListExecute(query.MerchantCode, lastGameRecorDirectory);
                    if (getFileList == null)
                    {
                        return new()
                        {
                            Success = false,
                        };
                    }
                    if (!string.IsNullOrEmpty(lastGameRecorFile))
                    {
                        var index = Array.IndexOf(getFileList, lastGameRecorFile);
                        if (index != -1)
                        {
                            getFileList = getFileList.Where((val, idx) => idx > index).ToArray();
                        }
                    }
                    else
                    {
                        getFileList = new[] { getFileList.Last() };
                    }

                    lastGameRecorFile = await ProcessFileListAsync(gameProviderId, externalGameId, query.SubGP, query.MerchantCode, lastGameRecorDirectory, getFileList).ConfigureAwait(false);
                    var day = DateTime.Parse($"{lastGameRecorDirectory.Substring(0, 4)}-{lastGameRecorDirectory.Substring(4, 2)}-{lastGameRecorDirectory.Substring(6, 2)}");
                    if (day.AddDays(1) <= nowDay)
                    {
                        lastGameRecorDirectory = day.AddDays(1).ToString("yyyyMMdd");
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return new()
            {
                Success = true,
                LastGameRecorDirectory = lastGameRecorDirectory,
                LastGameRecorFile = lastGameRecorFile
            };
        }

        private async Task<string> ProcessFileListAsync(Guid gameProviderId, string externalGameId, string subGP, string merchantCode, string lastGameRecorDirectory, string[] fileList)
        {
            _settledFileContentMappers.TryGetValue(subGP, out var fileContentExecute);
            string lastGameRecorFile = string.Empty;

            foreach (var file in fileList)
            {
                var response = await fileContentExecute(merchantCode, lastGameRecorDirectory, file).ConfigureAwait(false);
                if (response == null)
                {
                    return lastGameRecorFile;
                }
                var tasks = new List<Task>();
                foreach (var data in response.Datas)
                {
                    var getRound = await _gameRoundsProcessor.GetExternalRoundIdByExternalGameIdAsync(gameProviderId, data.OrderNum, externalGameId, data.Username).ConfigureAwait(false);
                    if (getRound == null)
                    {
                        continue;
                    }
                    await _gameRoundsProcessor.UpdateRoundDetailAsync(getRound.Id, JsonSerializer.Serialize(data)).ConfigureAwait(false);
                }
                await Task.WhenAll(tasks);
                lastGameRecorFile = file;
            }
            return lastGameRecorFile;
        }
    }
}
