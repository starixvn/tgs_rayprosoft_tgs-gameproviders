namespace TGS.GameProviders.Core.WMCasino
{
    public interface IWMCasinoUtility
    {
        string ToWMCasinoPlayerId(string playerCode, string prefix, string suffix);
    }
}
