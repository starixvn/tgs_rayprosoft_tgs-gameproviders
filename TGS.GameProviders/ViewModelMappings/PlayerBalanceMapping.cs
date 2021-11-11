using AutoMapper;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.KY.ApplicationServices;
using TGS.GameProviders.Core.LY.ApplicationServices;
using TGS.GameProviders.Core.VR.ApplicationServices;
using TGS.GameProviders.Core.WGHorse.ApplicationServices;
using TGS.GameProviders.Core.WGSports.ApplicationServices;
using TGS.GameProviders.ViewModels;

namespace TGS.GameProviders.ViewModelMappings
{
    public class PlayerBalanceMapping : Profile
    {
        public PlayerBalanceMapping()
        {
            CreateMap<PlayerBalanceResult, PlayerBalanceResponse>()
                .ReverseMap();
            CreateMap<PlayerBalanceRequest, KYPlayerBalanceQuery>()
                .ReverseMap();
            CreateMap<PlayerBalanceRequest, VRPlayerBalanceQuery>()
                .ReverseMap();
            CreateMap<PlayerBalanceRequest, WGSportsPlayerBalanceQuery>()
                .ReverseMap();
            CreateMap<PlayerBalanceRequest, WGHorsePlayerBalanceQuery>()
                .ReverseMap();
            CreateMap<PlayerBalanceRequest, LYPlayerBalanceQuery>()
                .ReverseMap();
        }
    }
}
