namespace TGS.GameProviders.Core.BigGaming
{
    public interface IBigGamingUtility
    {
        public string GetHashMD5(string plainText);

        public string GetSecretCode(string agentLoginIdPassword);
    }
}
