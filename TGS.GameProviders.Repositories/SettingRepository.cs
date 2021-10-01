using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TGS.GameProviders.Domains.SettingConfiguration;
using TGS.GameProviders.Repositories.Entities;

namespace TGS.GameProviders.Repositories
{
    internal sealed class SettingRepository : ISettingRepository
    {
        private readonly TgsContext _context;
        private readonly ILogger<SettingRepository> _logger;

        public SettingRepository(
           ILogger<SettingRepository> logger,
           TgsContext context
           )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<TConfiguration?> GetConfigurationAsync<TConfiguration>(string key) where TConfiguration : PluggableConfigurationBase
        {
            var data = await _context.Settings.FirstOrDefaultAsync(x => x.Key == key).ConfigureAwait(false);
            if (data == null)
            {
                return default;
            }
            return JsonSerializer.Deserialize<TConfiguration>(data.Value);
        }

        public Task<Setting> GetSettingAsync<TConfiguration>(string key)
            => _context.Settings.FirstOrDefaultAsync(x => x.Key == key);
    }
}
