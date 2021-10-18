using System;
using System.Reflection;
using TGS.GameProviders.GameRecord.Core;
using TGS.GameProviders.GameRecord.Core.Processors;
using TGS.GameProviders.TCG.FTPApi.Client;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class GameRecordCoreConfigurationExtensions
    {
        public static IServiceCollection AddGameRecordCoreConfiguration(
            this IServiceCollection services,
            Action<TcgFtpServiceClientOptions, IServiceProvider> ftpServiceClientConfigure)
        {
            services
                .RegisterAllServiceUtilityTypes(
                Assembly.GetExecutingAssembly(),
                ServiceLifetime.Transient);

            services.AddTcgFtpServiceClient(ftpServiceClientConfigure);

            return services
                .AddTransient<IGameRecordProcessor, GameRecordProcessor>()
                .AddTransient<IGameRecordProcessorSuccess, GameRecordProcessorSuccess>()
                .AddTransient<IGameRecordProcessorFailed, GameRecordProcessorFailed>();
        }
    }
}
