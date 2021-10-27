using AutoMapper;
using TGS.GameProviders.Core.KY.ApplicationServices;
using TGS.GameProviders.ViewModels;

namespace TGS.GameProviders.ViewModelMappings
{
    public class PlayerFundCheckingMapping : Profile
    {
        public PlayerFundCheckingMapping()
        {
            CreateMap<PlayerFundCheckingRequest, KYPlayerFundCheckingQuery>()
                .ReverseMap();
        }
    }
}
