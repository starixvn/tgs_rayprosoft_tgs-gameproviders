using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.KY.ApplicationServices;

namespace TGS.GameProviders.Controllers.GameRecordList
{
    public class KYController : GameRecordListController<KYGameRecordListQuery, KYGameRecordListQuery>
    {
        private readonly ILogger<KYController> _logger;
        private readonly IMapper _mapper;

        public KYController(
            ILogger<KYController> logger,
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
        ///             "startTime": "2021-08-12T05:50:00+00:00",
        ///             "endTime": "2021-08-12T06:00:00+00:00"
        ///         }
        /// </remarks>
        public override async ValueTask<GameRecordListResult?> GameRecordListAsync(
                    [FromServices] IGetService<KYGameRecordListQuery, GameRecordListResult> getService,
                    KYGameRecordListQuery request)
        {
            var result = await getService.GetAsync(request).ConfigureAwait(false);
            return result == null ? null : result;
        }
    }
}
