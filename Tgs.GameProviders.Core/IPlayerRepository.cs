using System.Threading.Tasks;
using Tgs.GameProviders.QueryServer.Models;

namespace Tgs.GameProviders.Core;

public interface IPlayerRepository
{
    Task<PlayerModel> GetAsync(string playerCode);
}
