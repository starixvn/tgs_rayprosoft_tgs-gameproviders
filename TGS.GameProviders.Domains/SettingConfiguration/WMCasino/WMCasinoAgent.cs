namespace TGS.GameProviders.Domains.SettingConfiguration.WMCasino
{
    public record WMCasinoAgent
    {
        public string AgentId { get; init; } = default!;

        public string CurrencyCode { get; init; } = default!;

        public string Prefix { get; init; } = default!;

        public string Signature { get; init; } = default!;

        public bool Status { get; init; } = default!;

        public string Suffix { get; init; } = default!;
    }
}
