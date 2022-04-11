using AutoMapper;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.Avia.ApplicationServices;
using Tgs.GameProviders.Core.KY.ApplicationServices;
using Tgs.GameProviders.Core.LY.ApplicationServices;
using Tgs.GameProviders.Core.VR.ApplicationServices;
using Tgs.GameProviders.Core.WGHorse.ApplicationServices;
using Tgs.GameProviders.Core.WGSports.ApplicationServices;
using Tgs.GameProviders.ViewModels;

namespace Tgs.GameProviders.ViewModelMappings
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
