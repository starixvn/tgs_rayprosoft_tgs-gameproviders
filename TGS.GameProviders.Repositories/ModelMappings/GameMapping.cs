using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using TGS.GameProviders.Repositories.Domains;
using TGS.GameProviders.Repositories.Domains.Models.Game;
using TGS.GameProviders.Repositories.Entities;

namespace TGS.GameProviders.Repositories.ModelMappings
{
    public class GameMapping : Profile
    {
        public GameMapping()
        {
            CreateMap<Game, GetGame>()
                .ForMember(trigger => trigger.Type, source => source.MapFrom(o => (GameType)o.Type))
                .ForMember(trigger => trigger.AvailableCurrencies, source => source.MapFrom(s => s.AvailableCurrencies.Split(',', StringSplitOptions.RemoveEmptyEntries)))
                .ReverseMap();

        }
    }

}
