using TGS.GameProviders.Core.WMCasino;

namespace TGS.GameProviders.Services.WMCasino
{
    public class WMCasinoUtility : IWMCasinoUtility
    {
        public string ToWMCasinoPlayerId(string playerCode, string prefix, string suffix)
            => $"{prefix}_{playerCode}_{suffix}";
    }
}
