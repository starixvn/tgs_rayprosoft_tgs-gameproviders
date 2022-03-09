using System;

namespace TGS.GameProviders.Core.DreamGame.Models
{
    public record LoginResult(bool Success = false, int CodeId = 501, Uri Url = null);
}
