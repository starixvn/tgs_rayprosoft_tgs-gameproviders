using AutoMapper;
using TGS.GameProviders.Core.Repositories.Models.Player;
using TGS.GameProviders.Repositories.Entities;

namespace TGS.GameProviders.Repositories.ModelMappings
{
    public class PlayerMapping : Profile
    {
        public PlayerMapping()
        {
            CreateMap<Player, GetPlayer>()
                .ReverseMap();
        }
    }
}
