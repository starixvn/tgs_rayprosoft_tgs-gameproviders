using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Domains.SettingConfiguration;
using TGS.GameProviders.Repositories.Entities;

namespace TGS.GameProviders.Repositories
{
    internal class SettingRepository : ISettingRepository
    {
        private readonly TgsContext _context;

        public SettingRepository(TgsContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<TConfiguration?> GetConfigurationAsync<TConfiguration>(string key) where TConfiguration : PluggableConfigurationBase
        {
            var data = await _context.Settings.FirstOrDefaultAsync(x => x.Key == key).ConfigureAwait(false);
            return data == null ? default : JsonSerializer.Deserialize<TConfiguration>(data.Value);
        }

        public Task<Setting> GetSettingAsync<TConfiguration>(string key)
            => _context.Settings.FirstOrDefaultAsync(x => x.Key == key);
    }
}
