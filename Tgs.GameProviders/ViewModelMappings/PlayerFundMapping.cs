using AutoMapper;
using Tgs.GameProviders.Core;
using Tgs.GameProviders.Core.Avia.ApplicationServices;
using Tgs.GameProviders.Core.Joker.ApplicationServices;
using Tgs.GameProviders.Core.KY.ApplicationServices;
using Tgs.GameProviders.Core.LY.ApplicationServices;
using Tgs.GameProviders.Core.PragmaticPlayTransfer.ApplicationServices;
using Tgs.GameProviders.Core.VR.ApplicationServices;
using Tgs.GameProviders.Core.WGHorse.ApplicationServices;
using Tgs.GameProviders.Core.WGSports.ApplicationServices;
using Tgs.GameProviders.ViewModels;

namespace Tgs.GameProviders.ViewModelMappings;

public class PlayerFundMapping : Profile
{
    public PlayerFundMapping()
    {
        _ = CreateMap<PlayerFundResult, PlayerFundResponse>()
            .ReverseMap();

        _ = CreateMap<PlayerFundRequest, KYPlayerFundOutQuery>()
            .ReverseMap();
        _ = CreateMap<PlayerFundRequest, KYPlayerFundInQuery>()
            .ReverseMap();

        _ = CreateMap<PlayerFundRequest, VRPlayerFundOutQuery>()
            .ReverseMap();
        _ = CreateMap<PlayerFundRequest, VRPlayerFundInQuery>()
            .ReverseMap();

        _ = CreateMap<PlayerFundRequest, WGSportsPlayerFundOutQuery>()
            .ReverseMap();
        _ = CreateMap<PlayerFundRequest, WGSportsPlayerFundInQuery>()
            .ReverseMap();

        _ = CreateMap<PlayerFundRequest, WGHorsePlayerFundOutQuery>()
            .ReverseMap();
        _ = CreateMap<PlayerFundRequest, WGHorsePlayerFundInQuery>()
            .ReverseMap();

        _ = CreateMap<PlayerFundRequest, LYPlayerFundOutQuery>()
            .ReverseMap();
        _ = CreateMap<PlayerFundRequest, LYPlayerFundInQuery>()
            .ReverseMap();
        _ = CreateMap<PlayerFundRequest, AviaPlayerFundOutQuery>()
            .ReverseMap();
        _ = CreateMap<PlayerFundRequest, AviaPlayerFundInQuery>()
            .ReverseMap();

        _ = CreateMap<PlayerFundRequest, JokerPlayerFundOutQuery>()
            .ReverseMap();
        _ = CreateMap<PlayerFundRequest, JokerPlayerFundInQuery>()
            .ReverseMap();

        _ = CreateMap<PlayerFundRequest, PragmaticPlayTransferPlayerFundOutQuery>()
            .ReverseMap();
        _ = CreateMap<PlayerFundRequest, PragmaticPlayTransferPlayerFundInQuery>()
            .ReverseMap();
    }
}
