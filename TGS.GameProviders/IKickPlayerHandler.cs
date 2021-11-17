using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core;

namespace TGS.GameProviders
{
    public interface IKickPlayerHandler<TCommand> where TCommand : KickPlayerCommand
    {
        [HttpDelete("kickplayer/{playerCode}")]
        ValueTask KickPlayerAsync([FromServices] ICommandService<TCommand> commandService, string playerCode);
    }
}
