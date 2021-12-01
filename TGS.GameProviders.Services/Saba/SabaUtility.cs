using TGS.GameProviders.Core.Saba;

namespace TGS.GameProviders.Services.Saba
{
    internal class SabaUtility : ISabaUtility
    {
        public string ToSabaPlayerId(string playerCode, bool isTestEnviroment)
            => isTestEnviroment ? $"{playerCode}_test" : playerCode;
    }
}