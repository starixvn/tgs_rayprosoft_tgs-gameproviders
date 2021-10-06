using TGS.GameProviders.GameRecordProcessors;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class GameRecordProcessorsConfigurationExtensions
    {
        public static IServiceCollection AddGameRecordProcessors(this IServiceCollection services)
            => services
                .AddTransient<IGameRecordProcessor, GameRecordProcessor>()
                .AddTransient<IGameRecordProcessorSuccess, GameRecordProcessorSuccess>()
                .AddTransient<IGameRecordProcessorFailed, GameRecordProcessorFailed>();
    }
}
