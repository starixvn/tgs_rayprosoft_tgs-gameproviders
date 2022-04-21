using Tgs.GameProviders.Core.IMESports;

namespace Tgs.GameProviders.Services.IMESports
{
    internal class IMESportsUtility : IIMESportsUtility
    {
        public string ToIMESportsPlayerId(string playerCode, string prefix)
            => $"{prefix}_{playerCode}";
    }
}
