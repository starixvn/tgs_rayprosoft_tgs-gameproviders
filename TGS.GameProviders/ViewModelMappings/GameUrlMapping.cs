using AutoMapper;
using TGS.GameProviders.Core;
using TGS.GameProviders.Core.KY.ApplicationServices;
using TGS.GameProviders.ViewModels;

namespace TGS.GameProviders.ViewModelMappings
{
    public class GameUrlMapping : Profile
    {
        public GameUrlMapping()
        {
            CreateMap<GameUrlResult, GameUrlResponse>()
             .ReverseMap();
            CreateMap<GameUrlRequest, KYGameUrlQuery>()
                .ReverseMap();
        }
    }
}
