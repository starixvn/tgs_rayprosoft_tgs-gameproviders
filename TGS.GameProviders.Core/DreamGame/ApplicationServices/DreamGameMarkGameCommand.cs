using System.Collections.Generic;

namespace Tgs.GameProviders.Core.DreamGame.ApplicationServices;

public record DreamGameMarkGameCommand(IEnumerable<string> externalRoundIds);
