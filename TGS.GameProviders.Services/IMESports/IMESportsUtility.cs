using TGS.GameProviders.Core.IMESports;

namespace TGS.GameProviders.Services.IMESports
{
    internal class IMESportsUtility : IIMESportsUtility
    {
        public string ToIMESportsPlayerId(string playerCode, string prefix)
            => $"{prefix}_{playerCode}";
    }
}