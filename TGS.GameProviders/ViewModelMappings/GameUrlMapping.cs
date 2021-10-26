using AutoMapper;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.KY.ApplicationServices;
using TGS.GameProviders.Core.SexyGaming.ApplicationServices;
using TGS.GameProviders.Core.Skywind.ApplicationServices;
using TGS.GameProviders.Core.TCG.ApplicationServices;
using TGS.GameProviders.Core.VR.ApplicationServices;
using TGS.GameProviders.Core.WGHorse.ApplicationServices;
using TGS.GameProviders.Core.WGSports.ApplicationServices;
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
        }
    }
}
