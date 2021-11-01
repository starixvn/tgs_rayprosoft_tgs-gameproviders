using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.WGSports.ApplicationServices;

namespace TGS.GameProviders.Controllers.GameRecordList
{
    public class WGFMController : GameRecordListController<WGSportsGameRecordListQuery, WGSportsGameRecordListQuery>
    {
        private readonly ILogger<WGFMController> _logger;
        private readonly IMapper _mapper;

        public WGFMController(
            ILogger<WGFMController> logger,
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
        ///             "isSettlement": true,
        ///             "startTime": "2021-10-26T03:34:00-04:00",
        ///             "endTime": "2021-10-26T03:44:00-04:00"
        ///         }
        /// </remarks>
        public override async ValueTask<GameRecordListResult?> GameRecordListAsync(
                [FromServices] IGetService<WGSportsGameRecordListQuery, GameRecordListResult> getService,
                WGSportsGameRecordListQuery request)
        {
            var result = await getService.GetAsync(request).ConfigureAwait(false);
            return result == null ? null : result;
        }
    }
}
