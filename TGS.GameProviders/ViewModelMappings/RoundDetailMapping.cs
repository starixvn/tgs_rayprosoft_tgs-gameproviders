using AutoMapper;
using TGS.GameProviders.Core.BigGaming.ApplicationServices;
using TGS.GameProviders.ViewModels;

namespace TGS.GameProviders.ViewModelMappings
{
    public class RoundDetailMapping : Profile
    {
        public RoundDetailMapping()
        {
            CreateMap<RoundDetailRequest, BigGamingRoundDetailQuery>()
                .ReverseMap();
        }
    }
}
