using AutoMapper;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.AllBet.ApplicationServices;
using Tgs.GameProviders.Core.AsiaGaming.ApplicationServices;
using Tgs.GameProviders.Core.Avia.ApplicationServices;
using Tgs.GameProviders.Core.BigGaming.ApplicationServices;
using Tgs.GameProviders.Core.Card365.ApplicationServices;
using Tgs.GameProviders.Core.DreamGame.ApplicationServices;
using Tgs.GameProviders.Core.Evolution.ApplicationServices;
using Tgs.GameProviders.Core.Habanero.ApplicationServices;
using Tgs.GameProviders.Core.IMESports.ApplicationServices;
using Tgs.GameProviders.Core.JDB.ApplicationServices;
using Tgs.GameProviders.Core.Jili.ApplicationServices;
using Tgs.GameProviders.Core.KY.ApplicationServices;
using Tgs.GameProviders.Core.LY.ApplicationServices;
using Tgs.GameProviders.Core.PragmaticPlay.ApplicationServices;
using Tgs.GameProviders.Core.Saba.ApplicationServices;
using Tgs.GameProviders.Core.SBO.ApplicationServices;
using Tgs.GameProviders.Core.SexyGaming.ApplicationServices;
using Tgs.GameProviders.Core.Skywind.ApplicationServices;
using Tgs.GameProviders.Core.TCG.ApplicationServices;
using Tgs.GameProviders.Core.VR.ApplicationServices;
using Tgs.GameProviders.Core.WGHorse.ApplicationServices;
using Tgs.GameProviders.Core.WGSports.ApplicationServices;
using Tgs.GameProviders.Core.WMCasino.ApplicationServices;
using Tgs.GameProviders.ViewModels;

namespace Tgs.GameProviders.ViewModelMappings
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
