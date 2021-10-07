using AutoMapper;
using TGS.GameProviders.Repositories.Domains;
using TGS.GameProviders.Repositories.Domains.Models.FailedGameRecordOperation;
using TGS.GameProviders.Repositories.Entities;

namespace TGS.GameProviders.Repositories.ModelMappings
{
    public class FailedGameRecordOperationMapping : Profile
    {
        public FailedGameRecordOperationMapping()
        {
            CreateMap<FailedGameRecordOperation, GetFailedGameRecordOperation>()
                .ForMember(trigger => trigger.AutoStatus, source => source.MapFrom(o => (FailedGameRecordOperationAutoStatus)o.AutoStatus))
                .ForMember(trigger => trigger.ProcesStatus, source => source.MapFrom(o => (FailedGameRecordOperationProcesStatus)o.ProcesStatus))
                .ReverseMap();
            CreateMap<AddFailedGameRecordOperation, FailedGameRecordOperation>()
                .ForMember(trigger => trigger.AutoStatus, source => source.MapFrom(o => (short)o.AutoStatus))
                .ForMember(trigger => trigger.ProcesStatus, source => source.MapFrom(o => (short)o.ProcesStatus))
                .ForMember(trigger => trigger.CreatedOn, source => source.MapFrom(o => o.CreatedOn.UtcDateTime))
                .ForMember(trigger => trigger.UpdatedOn, source => source.MapFrom(o => o.UpdatedOn.UtcDateTime))
                .ReverseMap();
        }
    }
}
