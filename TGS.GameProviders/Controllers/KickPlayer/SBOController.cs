using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Core.SBO.ApplicationServices;
using TGS.GameProviders.ViewModels;
using Tgs.ServiceUtility;

namespace TGS.GameProviders.Controllers.KickPlayer
{
    public class SBOController : KickPlayerController<SBOKickPlayerCommand>
    {
        private readonly ILogger<SBOController> _logger;
        private readonly IMapper _mapper;

        public SBOController(
            ILogger<SBOController> logger,
            IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
        }

        public override ValueTask KickPlayerAsync(
            [FromServices] ICommandService<SBOKickPlayerCommand> commandService,
            KickPlayerRequest request)
            => commandService.ExecuteAsync(_mapper.Map<SBOKickPlayerCommand>(request));
    }
}