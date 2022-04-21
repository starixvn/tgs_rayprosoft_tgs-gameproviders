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
            CreateMap<PlayerFundCheckingRequest, AviaPlayerFundCheckingQuery>()
                .ReverseMap();
        }
    }
}
