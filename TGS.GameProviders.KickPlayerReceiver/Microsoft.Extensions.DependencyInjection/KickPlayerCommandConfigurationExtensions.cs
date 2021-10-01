using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class KickPlayerCommandConfigurationExtensions
    {
        public static IServiceCollection AddKickPlayerHandles(this IServiceCollection services)
            => services
             .RegisterAllServiceUtilityTypes(
                    Assembly.GetExecutingAssembly(),
                    ServiceLifetime.Transient);
    }
}
