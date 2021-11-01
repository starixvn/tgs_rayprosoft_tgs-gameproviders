using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.WGHorse.ApplicationServices;

namespace TGS.GameProviders.Controllers.GameRecordList
{
    public class WGVIAController : GameRecordListController<WGHorseGameRecordListQuery, WGHorseGameRecordListQuery>
    {
        private readonly ILogger<WGVIAController> _logger;
        private readonly IMapper _mapper;

        public WGVIAController(
            ILogger<WGVIAController> logger,
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
        ///             "startTime": "2021-08-18T00:00:00+00:00",
        ///             "endTime": "2021-08-19T00:00:00+00:00",
        ///             "page": 1,
        ///             "rows": 500
        ///         }
        /// </remarks>
        public override async ValueTask<GameRecordListResult?> GameRecordListAsync(
                    [FromServices] IGetService<WGHorseGameRecordListQuery, GameRecordListResult> getService,
                    WGHorseGameRecordListQuery request)
        {
            var result = await getService.GetAsync(request).ConfigureAwait(false);
            return result == null ? null : result;
        }
    }
}
