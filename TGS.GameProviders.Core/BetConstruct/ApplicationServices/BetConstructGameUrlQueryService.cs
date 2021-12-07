using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.Core.Infrastructures.Security;
using Tgs.ServiceUtility;

namespace TGS.GameProviders.Core.BetConstruct.ApplicationServices
{
    public class BetConstructGameUrlQueryService : IGetService<BetConstructGameUrlQuery, GameUrlResult>
    {
        private readonly ITokenProvider _tokenProvider;
        private readonly IBetConstructService _service;

        public BetConstructGameUrlQueryService(
            ITokenProvider tokenProvider,
            IBetConstructService service)
        {
            _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async ValueTask<GameUrlResult> GetAsync(BetConstructGameUrlQuery query, CancellationToken cancellationToken = default)
        {
            var setting = await _service.GetSettingConfigurationAsync();
            var userId = setting.PartnerId;

            var authToken = await _tokenProvider.EncryptAsync(query.PlayerCode);

            var lang = query.Language switch
            {
                "zh-CN" => "chi",
                "zh-TW" => "chi",
                "th-TH" => "tha",
                "vi-VN" => "vnm",
                _ => "eng"
            };

            query.LobbyDomain = "https://devsportsbook.srvttt.com";

            var url = $"{query.LobbyDomain}/js/partnerinit.js?containerID=bcsportsbookcontainer&UserId={userId}&AuthToken={authToken}&lang={lang}&sport=1";

            return new GameUrlResult
            {
                Url = new Uri(url)
            };
        }
    }
}