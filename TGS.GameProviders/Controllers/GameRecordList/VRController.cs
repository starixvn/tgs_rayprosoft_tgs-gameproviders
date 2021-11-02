using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.VR.ApplicationServices;

namespace TGS.GameProviders.Controllers.GameRecordList
{
    public class VRController : GameRecordListController<VRGameRecordListQuery, VRGameRecordListQuery>
    {
        private readonly ILogger<VRController> _logger;
        private readonly IMapper _mapper;

        public VRController(
            ILogger<VRController> logger,
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
        ///             "startTime": "2021-10-28T03:50:00+08:00",
        ///             "endTime": "2021-10-28T04:00:00+08:00",
        ///             "isUpdateTime": true,
        ///             "page": 1,
        ///             "rows": 500
        ///         }
        /// </remarks>
        public override async ValueTask<GameRecordListResult?> GameRecordListAsync(
                        [FromServices] IGetService<VRGameRecordListQuery, GameRecordListResult> getService,
                        VRGameRecordListQuery request)
        {
            var result = await getService.GetAsync(request).ConfigureAwait(false);
            return result == null ? null : result;
        }
    }
}
