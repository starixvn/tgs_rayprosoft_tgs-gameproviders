using AutoMapper;
using TGS.GameProviders.Core.Repositories.Models.Rounds;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Repositories.Entities;

namespace TGS.GameProviders.Repositories.ModelMappings
{
    public class RoundMapping : Profile
    {
        public RoundMapping()
        {
            CreateMap<Round, RoundModel>()
                .ForMember(trigger => trigger.Status, source => source.MapFrom(o => (RoundStatus)o.Status))
                .ForMember(trigger => trigger.ClientStatus, source => source.MapFrom(o => (ClientRoundStatus)o.ClientStatus))
                .ReverseMap();
        }
    }
}
