using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.BigGaming.ApplicationServices;

namespace TGS.GameProviders.Controllers.GameRecordList
{

    public class BGController : GameRecordListController<BigGamingFishingGameRecordListQuery, BigGamingFishingGameRecordListQuery>
    {
        private readonly ILogger<BGController> _logger;
        private readonly IMapper _mapper;

        public BGController(
            ILogger<BGController> logger,
            IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpPost("FishingGame")]
        public override async ValueTask<GameRecordListResult?> GameRecordListAsync(
            [FromServices] IGetService<BigGamingFishingGameRecordListQuery, GameRecordListResult> getService,
            BigGamingFishingGameRecordListQuery request)
        {
            var result = await getService.GetAsync(request).ConfigureAwait(false);
            return result == null ? null : result;
        }
    }
}
