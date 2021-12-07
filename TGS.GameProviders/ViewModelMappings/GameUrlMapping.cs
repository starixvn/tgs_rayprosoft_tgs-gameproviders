using AutoMapper;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.AsiaGaming.ApplicationServices;
using TGS.GameProviders.Core.BetConstruct.ApplicationServices;
using TGS.GameProviders.Core.BigGaming.ApplicationServices;
using TGS.GameProviders.Core.Evolution.ApplicationServices;
using TGS.GameProviders.Core.Habanero.ApplicationServices;
using TGS.GameProviders.Core.IMESports.ApplicationServices;
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
            CreateMap<GameUrlResult, GameUrlResponse>()
                .ReverseMap();
            CreateMap<GameUrlRequest, KYGameUrlQuery>()
                .ReverseMap();
            CreateMap<GameUrlRequest, WGSportsGameUrlQuery>()
                .ReverseMap();
            CreateMap<GameUrlRequest, WGHorseGameUrlQuery>()
                .ReverseMap();
            CreateMap<GameUrlRequest, VRGameUrlQuery>()
                .ReverseMap();
            CreateMap<GameUrlRequest, TCGGameUrlQuery>()
                .ReverseMap();
            CreateMap<GameUrlRequest, SkywindGameUrlQuery>()
                .ReverseMap();
            CreateMap<GameUrlRequest, SexyGamingGameUrlQuery>()
                .ReverseMap();
            CreateMap<GameUrlRequest, EvolutionGameUrlQuery>()
                .ReverseMap();
            CreateMap<GameUrlRequest, BigGamingGameUrlQuery>()
                .ReverseMap();
            CreateMap<GameUrlRequest, LYGameUrlQuery>()
                .ReverseMap();
            CreateMap<GameUrlRequest, SBOGameUrlQuery>()
                .ReverseMap();
            CreateMap<GameUrlRequest, WMCasinoGameUrlQuery>()
                .ReverseMap();
            CreateMap<GameUrlRequest, AsiaGamingGameUrlQuery>()
                .ReverseMap();
            CreateMap<GameUrlRequest, PragmaticPlayGameUrlQuery>()
                .ReverseMap();
            CreateMap<GameUrlRequest, IMESportsGameUrlQuery>()
                .ReverseMap();
            CreateMap<GameUrlRequest, SabaGameUrlQuery>()
                .ReverseMap();
            CreateMap<GameUrlRequest, HabaneroGameUrlQuery>()
                .ReverseMap();
            CreateMap<GameUrlRequest, BetConstructGameUrlQuery>()
                .ReverseMap();
        }
    }
}