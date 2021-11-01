using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.ServiceUtility;

namespace TGS.GameProviders.KickPlayer.Core.ApplicationServices.SBO
{
    public class SBOKickPlayerCommandService : ICommandService<SBOPlayerKickCommand>
    {
        public async ValueTask ExecuteAsync(SBOPlayerKickCommand command, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();
    }
}