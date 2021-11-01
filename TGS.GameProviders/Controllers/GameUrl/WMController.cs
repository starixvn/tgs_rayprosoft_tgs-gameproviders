using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.WMCasino.ApplicationServices;
using TGS.GameProviders.ViewModels;
using Tgs.ServiceUtility;

namespace TGS.GameProviders.Controllers.GameUrl
{
    public class WMController : GameUrlController<WMCasinoGameUrlQuery>
    {
        private readonly ILogger<WMController> _logger;
        private readonly IMapper _mapper;

        public WMController(
            ILogger<WMController> logger,
            IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public override async ValueTask<GameUrlResponse?> GameUrlAsync(
            IGetService<WMCasinoGameUrlQuery, GameUrlResult> getService,
            GameUrlRequest request)
        {
            var result = await getService.GetAsync(_mapper.Map<WMCasinoGameUrlQuery>(request)).ConfigureAwait(false);

            return result == null ? null : _mapper.Map<GameUrlResponse>(result);
        }
    }
}