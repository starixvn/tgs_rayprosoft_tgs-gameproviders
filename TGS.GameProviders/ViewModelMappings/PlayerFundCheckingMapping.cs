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
    public class PlayerFundCheckingMapping : Profile
    {
        public PlayerFundCheckingMapping()
        {
            CreateMap<PlayerFundCheckingResult, PlayerFundCheckingResponse>()
                .ReverseMap();
            CreateMap<PlayerFundCheckingRequest, KYPlayerFundCheckingQuery>()
                .ReverseMap();
            CreateMap<PlayerFundCheckingRequest, WGHorsePlayerFundCheckingQuery>()
                .ReverseMap();
            CreateMap<PlayerFundCheckingRequest, WGSportsPlayerFundCheckingQuery>()
                .ReverseMap();
            CreateMap<VRPlayerFundCheckingRequest, VRPlayerFundCheckingQuery>()
                .ReverseMap();
            CreateMap<PlayerFundCheckingRequest, LYPlayerFundCheckingQuery>()
                .ReverseMap();
        }
    }
}
