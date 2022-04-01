using System.Collections.Generic;

namespace TGS.GameProviders.Core.DreamGame.ApplicationServices
{
    public record DreamGameMarkGameCommand(IEnumerable<string> externalRoundIds);
}
