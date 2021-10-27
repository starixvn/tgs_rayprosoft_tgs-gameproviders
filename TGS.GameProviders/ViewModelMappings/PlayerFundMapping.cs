using AutoMapper;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.KY.ApplicationServices;
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
        }
    }
}
