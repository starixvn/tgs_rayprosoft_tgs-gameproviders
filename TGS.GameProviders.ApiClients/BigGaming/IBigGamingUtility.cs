namespace TGS.GameProviders.ApiClients.BigGaming
{
    public interface IBigGamingUtility
    {
        public string GetHashMD5(string plainText);

        public string GetSecretCode(string agentLoginIdPassword);
    }
}
