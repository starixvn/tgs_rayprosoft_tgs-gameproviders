using Tgs.GameProviders.Core.Saba;

namespace Tgs.GameProviders.Services.Saba
{
    internal class SabaUtility : ISabaUtility
    {
        public string ToSabaPlayerId(string playerCode, bool isTestEnvironment)
            => isTestEnvironment ? $"{playerCode}_test" : playerCode;
    }
}
