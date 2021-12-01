using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.GameProviders.Core.Repositories;
using TGS.GameProviders.Core.Repositories.Models.Player;
using TGS.GameProviders.Domains;
using TGS.GameProviders.Repositories.Entities;

namespace TGS.GameProviders.Repositories
{
    internal class PlayerRepository : IPlayerRepository
    {
        private readonly TgsContext _context;

        public PlayerRepository(
            TgsContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Task<PlayerModel> GetAsync(string playerCode)
            =>
            (from player in _context.Players.AsQueryable()
             join brand in _context.Brands.AsQueryable()
             on player.BrandId equals brand.Id
             join licensee in _context.Licensees.AsQueryable()
             on brand.LicenseeId equals licensee.Id
             join reseller in _context.Resellers.AsQueryable()
             on licensee.ResellerId equals reseller.Id
             where player.PlayerCode == playerCode
             select new PlayerModel(
                 reseller.Id,
                 reseller.Code,
                 licensee.Id,
                 licensee.Code,
                 brand.Id,
                 brand.Code,
                 player.PlayerCode,
                 player.IdNumber,
                 player.Username,
                 player.RealName,
                 player.DefaultWalletId,
                 player.DefaultWalletCurrencyCode,
                 (PlayerStatus)player.Status
                 ))
            .FirstOrDefaultAsync();

        public Task<PlayerModel> GetAsync(Guid brandId, string username)
        =>
            (from player in _context.Players.AsQueryable()
             join brand in _context.Brands.AsQueryable()
             on player.BrandId equals brand.Id
             join licensee in _context.Licensees.AsQueryable()
             on brand.LicenseeId equals licensee.Id
             join reseller in _context.Resellers.AsQueryable()
             on licensee.ResellerId equals reseller.Id
             where
             player.BrandId == brandId
             && player.Username == username
             select new PlayerModel(
                 reseller.Id,
                 reseller.Code,
                 licensee.Id,
                 licensee.Code,
                 brand.Id,
                 brand.Code,
                 player.PlayerCode,
                 player.IdNumber,
                 player.Username,
                 player.RealName,
                 player.DefaultWalletId,
                 player.DefaultWalletCurrencyCode,
                 (PlayerStatus)player.Status
                 ))
            .FirstOrDefaultAsync();

        public async Task<decimal?> GetPlayerBalanceAsync(string playerCode)
        {
            var walletId = await _context.Players.AsQueryable().Where(p => p.PlayerCode == playerCode)
                .Select(p => (Guid?)p.DefaultWalletId).FirstOrDefaultAsync().ConfigureAwait(false);

            return !walletId.HasValue
                ? null
                : await _context.Wallets.AsQueryable().Where(w => w.Id == walletId.Value).Select(w => w.Balance)
                .FirstOrDefaultAsync().ConfigureAwait(false);
        }
    }
}
