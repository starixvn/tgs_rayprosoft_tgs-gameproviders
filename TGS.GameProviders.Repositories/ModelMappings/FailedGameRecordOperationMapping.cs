using AutoMapper;
using TGS.GameProviders.Repositories.Domains.Models.FailedGameRecordOperation;
using TGS.GameProviders.Repositories.Entities;

namespace TGS.GameProviders.Repositories.ModelMappings
{
    public class FailedGameRecordOperationMapping : Profile
    {
        public FailedGameRecordOperationMapping()
        {
            CreateMap<FailedGameRecordOperation, GetFailedGameRecordOperation>();
            CreateMap<AddFailedGameRecordOperation, FailedGameRecordOperation>();
        }
    }
}
