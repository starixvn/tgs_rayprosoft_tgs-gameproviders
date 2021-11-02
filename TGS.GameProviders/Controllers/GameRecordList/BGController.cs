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

        /// <summary>
        /// Games the record list asynchronous.
        /// </summary>
        /// <param name="getService">The get service.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        /// <remarks>
        /// Example
        ///     Post
        ///         {
        ///             "currencyCode": "CNY",
        ///             "startTime": "2021-11-01T01:30:08.1413497+00:00",
        ///             "endTime": "2021-11-01T01:35:08.1413497+00:00",
        ///             "page": 1,
        ///             "rows": 500
        ///         }
        /// </remarks>
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
