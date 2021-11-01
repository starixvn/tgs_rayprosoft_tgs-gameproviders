using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.LY.ApplicationServices;

namespace TGS.GameProviders.Controllers.GameRecordList
{
    public class LYController : GameRecordListController<LYGameRecordListQuery, LYGameRecordListQuery>
    {
        private readonly ILogger<LYController> _logger;
        private readonly IMapper _mapper;

        public LYController(
            ILogger<LYController> logger,
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
        ///             "brandCode": "Default",
        ///             "currencyCode": "CNY",
        ///             "startTime": "2021-10-28T13:38:33+08:00",
        ///             "endTime": "2021-10-28T13:39:05+08:00"
        ///         }
        /// </remarks>
        public override async ValueTask<GameRecordListResult?> GameRecordListAsync(
                    [FromServices] IGetService<LYGameRecordListQuery, GameRecordListResult> getService,
                    LYGameRecordListQuery request)
        {
            var result = await getService.GetAsync(request).ConfigureAwait(false);
            return result == null ? null : result;
        }
    }
}
