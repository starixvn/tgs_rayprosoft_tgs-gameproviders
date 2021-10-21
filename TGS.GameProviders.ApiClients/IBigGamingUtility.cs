namespace TGS.GameProviders.ApiClients
{
    public interface IBigGamingUtility
    {
        public string GetHashMD5(string plainText);

        public string GetSecretCode(string agentLoginIdPassword);
    }
}
