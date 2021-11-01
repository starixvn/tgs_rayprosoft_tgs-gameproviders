namespace TGS.GameProviders.Core
{
    public interface IWMCasinoUtility
    {
        string ToWMCasinoPlayerId(string playerCode, string prefix, string suffix);
    }
}