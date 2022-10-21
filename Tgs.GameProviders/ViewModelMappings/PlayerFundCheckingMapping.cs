using AutoMapper;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.Avia.ApplicationServices;
using Tgs.GameProviders.Core.Joker.ApplicationServices;
using Tgs.GameProviders.Core.KY.ApplicationServices;
using Tgs.GameProviders.Core.LY.ApplicationServices;
using Tgs.GameProviders.Core.PragmaticPlayTransfer.ApplicationServices;
using Tgs.GameProviders.Core.VR.ApplicationServices;
using Tgs.GameProviders.Core.WGFmGaming.ApplicationServices;
using Tgs.GameProviders.Core.WGHorse.ApplicationServices;
using Tgs.GameProviders.ViewModels;

namespace Tgs.GameProviders.ViewModelMappings;

public class PlayerFundCheckingMapping : Profile
{
    public PlayerFundCheckingMapping()
    {
        _ = CreateMap<PlayerFundCheckingResult, PlayerFundCheckingResponse>()
              .ReverseMap();
        _ = CreateMap<PlayerFundCheckingRequest, KYPlayerFundCheckingQuery>()
              .ReverseMap();
        _ = CreateMap<PlayerFundCheckingRequest, WGHorsePlayerFundCheckingQuery>()
              .ReverseMap();
        _ = CreateMap<PlayerFundCheckingRequest, WGFmGamingPlayerFundCheckingQuery>()
              .ReverseMap();
        _ = CreateMap<VRPlayerFundCheckingRequest, VRPlayerFundCheckingQuery>()
              .ReverseMap();
        _ = CreateMap<PlayerFundCheckingRequest, LYPlayerFundCheckingQuery>()
              .ReverseMap();
        _ = CreateMap<PlayerFundCheckingRequest, AviaPlayerFundCheckingQuery>()
              .ReverseMap();
        _ = CreateMap<PlayerFundCheckingRequest, JokerPlayerFundCheckingQuery>()
            .ReverseMap();
        _ = CreateMap<PlayerFundCheckingRequest, PragmaticPlayTransferPlayerFundCheckingQuery>()
              .ReverseMap();
    }
}
