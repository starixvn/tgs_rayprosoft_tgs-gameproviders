using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tgs.ServiceUtility;
using TGS.GameProviders.Core;
using TGS.GameProviders.ViewModels;

namespace TGS.GameProviders
{
    public interface IKickPlayerHandler<TCommand> where TCommand : KickPlayerCommand
    {
        ValueTask KickPlayerAsync([FromServices] ICommandService<TCommand> commandService, KickPlayerRequest request);
    }
}
