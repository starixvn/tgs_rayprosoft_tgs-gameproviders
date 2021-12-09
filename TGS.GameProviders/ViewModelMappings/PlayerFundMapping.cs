using AutoMapper;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.Avia.ApplicationServices;
using TGS.GameProviders.Core.KY.ApplicationServices;
using TGS.GameProviders.Core.LY.ApplicationServices;
using TGS.GameProviders.Core.VR.ApplicationServices;
using TGS.GameProviders.Core.WGHorse.ApplicationServices;
using TGS.GameProviders.Core.WGSports.ApplicationServices;
using TGS.GameProviders.ViewModels;

namespace TGS.GameProviders.ViewModelMappings
{
    public class PlayerFundMapping : Profile
    {
        public PlayerFundMapping()
        {
            CreateMap<PlayerFundResult, PlayerFundResponse>()
                .ReverseMap();

            CreateMap<PlayerFundRequest, KYPlayerFundOutQuery>()
                .ReverseMap();
            CreateMap<PlayerFundRequest, KYPlayerFundInQuery>()
                .ReverseMap();

            CreateMap<PlayerFundRequest, VRPlayerFundOutQuery>()
                .ReverseMap();
            CreateMap<PlayerFundRequest, VRPlayerFundInQuery>()
                .ReverseMap();

            CreateMap<PlayerFundRequest, WGSportsPlayerFundOutQuery>()
                .ReverseMap();
            CreateMap<PlayerFundRequest, WGSportsPlayerFundInQuery>()
                .ReverseMap();

            CreateMap<PlayerFundRequest, WGHorsePlayerFundOutQuery>()
                .ReverseMap();
            CreateMap<PlayerFundRequest, WGHorsePlayerFundInQuery>()
                .ReverseMap();

            CreateMap<PlayerFundRequest, LYPlayerFundOutQuery>()
                .ReverseMap();
            CreateMap<PlayerFundRequest, LYPlayerFundInQuery>()
                .ReverseMap();
            CreateMap<PlayerFundRequest, AviaPlayerFundOutQuery>()
                .ReverseMap();
            CreateMap<PlayerFundRequest, AviaPlayerFundInQuery>()
                .ReverseMap();
        }
    }
}
