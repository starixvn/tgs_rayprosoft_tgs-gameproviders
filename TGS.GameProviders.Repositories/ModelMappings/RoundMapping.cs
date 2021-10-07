using System;
using AutoMapper;
using TGS.GameProviders.Repositories.Domains;
using TGS.GameProviders.Repositories.Domains.Models.Rounds;
using TGS.GameProviders.Repositories.Entities;

namespace TGS.GameProviders.Repositories.ModelMappings
{
    public class RoundMapping : Profile
    {
        public RoundMapping()
        {
            CreateMap<Round, GetRound>()
                .ForMember(trigger => trigger.Status, source => source.MapFrom(o => (RoundStatus)o.Status))
                .ForMember(trigger => trigger.ClientStatus, source => source.MapFrom(o => (ClientRoundStatus)o.ClientStatus))
                .ReverseMap();
            CreateMap<AddRound, Round>()
                .ForMember(trigger => trigger.Status, source => source.MapFrom(o => (short)o.Status))
                .ForMember(trigger => trigger.ClientStatus, source => source.MapFrom(o => (short)o.ClientStatus))
                .ForMember(trigger => trigger.CreatedOn, source => source.MapFrom(o => o.CreatedOn.UtcDateTime))
                .ForMember(trigger => trigger.ClosedOn, source => source.MapFrom(o => (o.ClosedOn.HasValue) ? o.ClosedOn.Value.UtcDateTime : (DateTime?)null))
                .ForMember(trigger => trigger.UpdatedOn, source => source.MapFrom(o => o.UpdatedOn.UtcDateTime))
                .ForMember(trigger => trigger.ExternalCreatedOn, source => source.MapFrom(o => (o.ExternalCreatedOn.HasValue) ? o.ExternalCreatedOn.Value.UtcDateTime : (DateTime?)null))
                .ForMember(trigger => trigger.ExternalClosedOn, source => source.MapFrom(o => (o.ExternalClosedOn.HasValue) ? o.ExternalClosedOn.Value.UtcDateTime : (DateTime?)null))
                .ReverseMap();
        }
    }
}
