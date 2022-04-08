using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.Core.Infrastructures.Security;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders.Core.BetConstruct.ApplicationServices
{
    public class BetConstructGameUrlQueryService : IGetService<BetConstructGameUrlQuery, GameUrlResult>
    {
        private readonly IBetConstructService _service;
        private readonly ITokenProvider _tokenProvider;

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

            var oddsType = "decimal";

            if (query.BrandCode is "HK28")
                oddsType = "hongkong";

            var lang = query.Language switch
            {
                "zh-CN" => "chi",
                "zh-TW" => "chi",
                "th-TH" => "tha",
                "vi-VN" => "vnm",
                _ => "eng"
            };

            var url = $"{query.LobbyDomain}/js/partnerinit.js?containerID=bcsportsbookcontainer&UserId={userId}&AuthToken={authToken}&lang={lang}&sport=1&oddsType={oddsType}";

            return new GameUrlResult
            {
                Url = new Uri(url)
            };
        }
    }
}
