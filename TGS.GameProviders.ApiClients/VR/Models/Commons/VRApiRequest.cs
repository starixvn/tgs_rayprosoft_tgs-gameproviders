namespace TGS.GameProviders.ApiClients.VR.Models.Commons
{
    public record VRApiRequest
    {
        public string version { get; init; }
        public string id { get; init; }
        public string data { get; init; }
    }
}
