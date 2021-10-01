namespace TGS.GameProviders.Repositories.Domains
{
    public static class GameProviderConfigurations
    {
        private static readonly string Template = "{0}Configuration";
        public static readonly string AllBet = string.Format(Template, GameProviderType.AllBet);
        public static readonly string AsiaGaming = string.Format(Template, GameProviderType.AsiaGaming);
        public static readonly string BigGaming = string.Format(Template, GameProviderType.BigGaming);
        public static readonly string Card365 = string.Format(Template, GameProviderType.Card365);
        public static readonly string Evolution = string.Format(Template, GameProviderType.Evolution);
        public static readonly string Habanero = string.Format(Template, GameProviderType.Habanero);
        public static readonly string IMESports = string.Format(Template, GameProviderType.IMESports);
        public static readonly string JDB = string.Format(Template, GameProviderType.JDB);
        public static readonly string KY = string.Format(Template, GameProviderType.KY);
        public static readonly string PragmaticPlay = string.Format(Template, GameProviderType.PragmaticPlay);
        public static readonly string Saba = string.Format(Template, GameProviderType.Saba);
        public static readonly string SBOBET = string.Format(Template, GameProviderType.SBOBET);
        public static readonly string SexyGaming = string.Format(Template, GameProviderType.SexyGaming);
        public static readonly string Skywind = string.Format(Template, GameProviderType.Skywind);
        public static readonly string TCG = string.Format(Template, GameProviderType.TCG);
        public static readonly string VR = string.Format(Template, GameProviderType.VR);
        public static readonly string WGHorse = string.Format(Template, GameProviderType.WGHorse);
        public static readonly string WGSports = string.Format(Template, GameProviderType.WGSports);
        public static readonly string WMCasino = string.Format(Template, GameProviderType.WMCasino);
    }
}
