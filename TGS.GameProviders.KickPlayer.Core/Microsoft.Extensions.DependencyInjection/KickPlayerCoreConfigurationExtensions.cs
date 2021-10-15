using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class GameRecordCoreConfigurationExtensions
    {
        public static IServiceCollection AddKickPlayerCoreConfiguration(this IServiceCollection services)
            => services
            .RegisterAllServiceUtilityTypes(
                Assembly.GetExecutingAssembly(),
                ServiceLifetime.Transient);
    }
}
