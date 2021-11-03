using System;
using AutoMapper;
using TGS.GameProviders.Core.Repositories.Models.Game;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Repositories.Entities;

namespace TGS.GameProviders.Repositories.ModelMappings
{
    public class GameMapping : Profile
    {
        public GameMapping()
        {
            CreateMap<Game, GetGame>()
                .ForMember(trigger => trigger.GameId, source => source.MapFrom(o => o.Id))
                .ForMember(trigger => trigger.Type, source => source.MapFrom(o => (GameType)o.Type))
                .ForMember(trigger => trigger.AvailableCurrencies, source => source.MapFrom(s => (string.IsNullOrEmpty(s.AvailableCurrencies)) ? null : s.AvailableCurrencies.Split(",", StringSplitOptions.RemoveEmptyEntries)))
                .ReverseMap();
        }
    }
}
