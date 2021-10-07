using System.Reflection;
using TGS.GameProviders.GameRecord.Core;
using TGS.GameProviders.GameRecord.Core.Processors;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class GameRecordCoreConfigurationExtensions
    {
        public static IServiceCollection AddGameRecordCoreConfiguration(this IServiceCollection services)
            => services
            .RegisterAllServiceUtilityTypes(
                Assembly.GetExecutingAssembly(),
                ServiceLifetime.Transient)
            .AddTransient<IGameRecordProcessor, GameRecordProcessor>()
            .AddTransient<IGameRecordProcessorSuccess, GameRecordProcessorSuccess>()
            .AddTransient<IGameRecordProcessorFailed, GameRecordProcessorFailed>();
    }
}
