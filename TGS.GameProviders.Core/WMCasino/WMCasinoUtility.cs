namespace TGS.GameProviders.Core.WMCasino
{
    public class WMCasinoUtility : IWMCasinoUtility
    {
        public string ToWMCasinoPlayerId(string playerCode, string prefix, string suffix)
            => $"{prefix}_{playerCode}_{suffix}";
    }
}