using AutoMapper;
using Tgs.GameProviders.Core.BigGaming.ApplicationServices;
using Tgs.GameProviders.ViewModels;

namespace Tgs.GameProviders.ViewModelMappings;

public class RoundDetailMapping : Profile
{
    public RoundDetailMapping()
    {
        CreateMap<RoundDetailRequest, BigGamingRoundDetailQuery>()
            .ReverseMap();
    }
}
