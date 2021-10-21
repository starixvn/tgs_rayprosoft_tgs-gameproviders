using System.Threading.Tasks;
using TGS.GameProviders.KickPlayer.Core.ApplicationServices.SBO;
using TGS.ServiceUtility;

namespace TGS.GameProviders.KickPlayer.ReceiverServer.Controllers
{
    public class SBOKickPlayerController : BaseController<SBOPlayerKickCommand>
    {
        public override ValueTask KickPlayerAsync(
            ICommandService<SBOPlayerKickCommand> commandService,
            SBOPlayerKickCommand command)
            => commandService.ExecuteAsync(command);
    }
}