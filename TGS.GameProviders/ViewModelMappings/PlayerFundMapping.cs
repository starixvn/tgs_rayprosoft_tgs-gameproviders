using AutoMapper;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.KY.ApplicationServices;
using TGS.GameProviders.Core.VR.ApplicationServices;
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
        }
    }
}
