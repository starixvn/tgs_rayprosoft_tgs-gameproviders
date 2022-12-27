using Microsoft.AspNetCore.Mvc;
using Tgs.GameProviders.Core;
using Tgs.ServiceUtility;

namespace Tgs.GameProviders;

public interface IKickPlayerHandler<TCommand> where TCommand : KickPlayerCommand
{
    [HttpDelete("kickplayer/{playerCode}")]
    ValueTask KickPlayerAsync([FromServices] ICommandService<TCommand> commandService, string playerCode);
}
