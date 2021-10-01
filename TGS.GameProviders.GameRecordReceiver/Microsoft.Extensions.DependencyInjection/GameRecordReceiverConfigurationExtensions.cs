using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace TGS.GameProviders.GameRecordReceiver.Microsoft.Extensions.DependencyInjection
{
    public static class GameRecordReceiverConfigurationExtensions
    {
        public static IServiceCollection AddGameRecordReceiver(this IServiceCollection services)
            => services
            .RegisterAllServiceUtilityTypes(
                Assembly.GetExecutingAssembly(),
                ServiceLifetime.Transient);
    }
}
