using System;

namespace Tgs.GameProviders.Core.IDNLive
{
    public record IdnLivePlayerInfo
    {
        public int Uid { get; set; }
        public Guid WalletId { get; set; }
        public string Nickname { get; set; }
        public string Currency { get; set; }
        public string Table { get; set; }
        public string Language { get; set; }
        public string ClientIP { get; set; }
    }
}
