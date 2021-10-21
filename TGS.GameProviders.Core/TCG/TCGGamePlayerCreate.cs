using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.ApiClients;
using TGS.GameProviders.ApiClients.TCG.Models;

namespace TGS.GameProviders.Core.TCG
{
    public class TCGGamePlayerCreate : ITCGGamePlayerCreate
    {
        private readonly ITCGApiClient _client;
        private readonly ILogger<TCGGamePlayerCreate> _logger;

        public TCGGamePlayerCreate(
            ILogger<TCGGamePlayerCreate> logger,
            ITCGApiClient client
            )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async ValueTask<bool> CreatePlayerAsync(Uri apiUrl, string merchantCode, string desKey, string sha256Key, string playerPassword, string playerCode)
        {
            var data = new TCGRegisterPlayerRequest()
            {
                UserName = playerCode,
                Password = playerPassword
            };

            var response = await _client.RegisterPlayerAsync(
                 apiUrl,
                 merchantCode,
                 desKey,
                 sha256Key,
                 data).ConfigureAwait(false);
            if (response.Status == 0)
            {
                return true;
            }

            _logger.Log(
                LogLevel.Warning,
             new EventId(1000, nameof(TCGGamePlayerCreate)),
             data,
             null,
             (state, ex) =>
              $"【{nameof(data)}】->{JsonSerializer.Serialize(state)}.{ Environment.NewLine}" +
              $"【DateTime】->{DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(+8))}.{ Environment.NewLine}" +
              $"【{nameof(response)}】->{JsonSerializer.Serialize(response)}.{ Environment.NewLine}" +
              $"【Message】->創建會員失敗"
              );
            return false;
        }
    }
}
