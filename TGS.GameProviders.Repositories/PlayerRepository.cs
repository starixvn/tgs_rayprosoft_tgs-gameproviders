using System;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TGS.GameProviders.Repositories.Domains.Models.Player;
using TGS.GameProviders.Repositories.Entities;

namespace TGS.GameProviders.Repositories
{
    internal class PlayerRepository : IPlayerRepository
    {
        private readonly TgsContext _context;
        private readonly IMapper _mapper;

        public PlayerRepository(
            TgsContext context,
            IMapper mapper
            )
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<GetPlayer?> GetPlayerAsync(string playerCode)
        {
            var player = await _context.Players
                .ProjectTo<GetPlayer>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(x => x.PlayerCode == playerCode)
                .ConfigureAwait(false);
            if (player == null)
            {
                return null;
            }
            var brands = await _context.Brands.FirstOrDefaultAsync(x => x.Id == player.BrandId).ConfigureAwait(false);
            player.LicenseeId = brands.LicenseeId;
            player.BrandCode = brands.Code;
            return player;
        }

        public Task<Player?> GetPlayerAsync(Guid brandId, string username)
            => _context.Players.FirstOrDefaultAsync(x => x.BrandId == brandId && x.Username == username);
    }
}
