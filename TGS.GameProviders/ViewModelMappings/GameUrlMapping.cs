using AutoMapper;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.AllBet.ApplicationServices;
using TGS.GameProviders.Core.AsiaGaming.ApplicationServices;
using TGS.GameProviders.Core.Avia.ApplicationServices;
using TGS.GameProviders.Core.BigGaming.ApplicationServices;
using TGS.GameProviders.Core.Card365.ApplicationServices;
using TGS.GameProviders.Core.DreamGame.ApplicationServices;
using TGS.GameProviders.Core.Evolution.ApplicationServices;
using TGS.GameProviders.Core.Habanero.ApplicationServices;
using TGS.GameProviders.Core.IMESports.ApplicationServices;
using TGS.GameProviders.Core.JDB.ApplicationServices;
using TGS.GameProviders.Core.Jili.ApplicationServices;
using TGS.GameProviders.Core.KY.ApplicationServices;
using TGS.GameProviders.Core.LY.ApplicationServices;
using TGS.GameProviders.Core.PragmaticPlay.ApplicationServices;
using TGS.GameProviders.Core.Saba.ApplicationServices;
using TGS.GameProviders.Core.SBO.ApplicationServices;
using TGS.GameProviders.Core.SexyGaming.ApplicationServices;
using TGS.GameProviders.Core.Skywind.ApplicationServices;
using TGS.GameProviders.Core.TCG.ApplicationServices;
using TGS.GameProviders.Core.VR.ApplicationServices;
using TGS.GameProviders.Core.WGHorse.ApplicationServices;
using TGS.GameProviders.Core.WGSports.ApplicationServices;
using TGS.GameProviders.Core.WMCasino.ApplicationServices;
using TGS.GameProviders.ViewModels;

namespace TGS.GameProviders.ViewModelMappings
{
    public class GameUrlMapping : Profile
    {
        public GameUrlMapping()
        {
            _ = CreateMap<GameUrlResult, GameUrlResponse>()
                .ReverseMap();
            _ = CreateMap<GameUrlRequest, KYGameUrlQuery>()
                .ReverseMap();
            _ = CreateMap<GameUrlRequest, WGSportsGameUrlQuery>()
                .ReverseMap();
            _ = CreateMap<GameUrlRequest, WGHorseGameUrlQuery>()
                .ReverseMap();
            _ = CreateMap<GameUrlRequest, VRGameUrlQuery>()
                .ReverseMap();
            _ = CreateMap<GameUrlRequest, TCGGameUrlQuery>()
                .ReverseMap();
            _ = CreateMap<GameUrlRequest, SkywindGameUrlQuery>()
                .ReverseMap();
            _ = CreateMap<GameUrlRequest, SexyGamingGameUrlQuery>()
                .ReverseMap();
            _ = CreateMap<GameUrlRequest, EvolutionGameUrlQuery>()
                .ReverseMap();
            _ = CreateMap<GameUrlRequest, BigGamingGameUrlQuery>()
                .ReverseMap();
            _ = CreateMap<GameUrlRequest, LYGameUrlQuery>()
                .ReverseMap();
            _ = CreateMap<GameUrlRequest, SBOGameUrlQuery>()
                .ReverseMap();
            _ = CreateMap<GameUrlRequest, WMCasinoGameUrlQuery>()
                .ReverseMap();
            _ = CreateMap<GameUrlRequest, AsiaGamingGameUrlQuery>()
                .ReverseMap();
            _ = CreateMap<GameUrlRequest, PragmaticPlayGameUrlQuery>()
                .ReverseMap();
            _ = CreateMap<GameUrlRequest, IMESportsGameUrlQuery>()
                .ReverseMap();
            _ = CreateMap<GameUrlRequest, SabaGameUrlQuery>()
                .ReverseMap();
            _ = CreateMap<GameUrlRequest, HabaneroGameUrlQuery>()
                .ReverseMap();
            _ = CreateMap<GameUrlRequest, AllBetGameUrlQuery>()
                .ReverseMap();
            _ = CreateMap<GameUrlRequest, AviaGameUrlQuery>()
                .ReverseMap();
            _ = CreateMap<GameUrlRequest, JiliGameUrlQuery>()
                .ReverseMap();
            _ = CreateMap<GameUrlRequest, Card365GameUrlQuery>()
                .ReverseMap();
            _ = CreateMap<GameUrlRequest, JDBGameUrlQuery>()
                .ReverseMap();
            _ = CreateMap<GameUrlRequest, DreamGameGameUrlQuery>()
                .ReverseMap();
        }
    }
}
