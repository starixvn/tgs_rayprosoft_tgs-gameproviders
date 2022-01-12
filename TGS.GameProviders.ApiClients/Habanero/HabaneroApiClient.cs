using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Core.Habanero;
using TGS.GameProviders.Core.Habanero.Models;
using Tgs.Logging;

namespace TGS.GameProviders.ApiClients.Habanero
{
    internal class HabaneroApiClient : IHabaneroApiClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<HabaneroApiClient> _logger;
        private readonly ITgsLog _tgsLog;

        public HabaneroApiClient(
            HttpClient client,
            ILogger<HabaneroApiClient> logger,
            ITgsLog tgsLog)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tgsLog = tgsLog ?? throw new ArgumentNullException(nameof(tgsLog));
        }

        public ValueTask<HabaneroLogoutResponse> LogoutAsync(Uri apiUrl, HabaneroLogoutRequest request)
            => throw new NotImplementedException();
    }
}