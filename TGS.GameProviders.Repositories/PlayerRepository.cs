using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Repositories.Domains.Models.Player;
using TGS.GameProviders.Repositories.Entities;
using TGS.GameProviders.Repositories.ModelMappings;

namespace TGS.GameProviders.Repositories
{
    internal class PlayerRepository : IPlayerRepository
    {
        private readonly TgsContext _context;
        private readonly ILogger<PlayerRepository> _logger;
        private readonly IMapper _mapper;

        public PlayerRepository(
            ILogger<PlayerRepository> logger,
            TgsContext context
            )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = new MapperConfiguration(configure => configure.AddProfile<PlayerMapping>()).CreateMapper();
        }

        public async Task<GetPlayer?> GetPlayerAsync(string playerCode)
        {
            var player = await _context.Players.Where(x => x.PlayerCode == playerCode).Select(x => _mapper.Map<GetPlayer>(x)).FirstOrDefaultAsync().ConfigureAwait(false);
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
