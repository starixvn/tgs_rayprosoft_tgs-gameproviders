using AutoMapper;
using TGS.GameProviders.Core.BigGaming.ApplicationServices;
using TGS.GameProviders.Core.KY.ApplicationServices;
using TGS.GameProviders.Core.SBO.ApplicationServices;
using TGS.GameProviders.Core.SexyGaming.ApplicationServices;
using TGS.GameProviders.Core.TCG.ApplicationServices;
using TGS.GameProviders.Core.VR.ApplicationServices;
using TGS.GameProviders.Core.WGHorse.ApplicationServices;
using TGS.GameProviders.Core.WGSports.ApplicationServices;
using TGS.GameProviders.ViewModels;

namespace TGS.GameProviders.ViewModelMappings
{
    public class KickPlayerMapping : Profile
    {
        public KickPlayerMapping()
        {
            CreateMap<KickPlayerRequest, BigGamingKickPlayerCommand>()
                .ReverseMap();
            CreateMap<KickPlayerRequest, KYKickPlayerCommand>()
                .ReverseMap();
            CreateMap<KickPlayerRequest, SexyGamingKickPlayerCommand>()
                .ReverseMap();
            CreateMap<KickPlayerRequest, TCGKickPlayerCommand>()
                .ReverseMap();
            CreateMap<KickPlayerRequest, VRKickPlayerCommand>()
                .ReverseMap();
            CreateMap<KickPlayerRequest, WGSportsKickPlayerCommand>()
                .ReverseMap();
            CreateMap<KickPlayerRequest, WGHorseKickPlayerCommand>()
                .ReverseMap();
            CreateMap<KickPlayerRequest, SBOKickPlayerCommand>()
                .ReverseMap();
        }
    }
}