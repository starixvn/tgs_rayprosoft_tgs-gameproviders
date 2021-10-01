using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using TGS.GameProviders.KickPlayerReceiver.ReceiveHandlers;

namespace TGS.GameProviders.KickPlayerReceiver
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TGS.GameProviders.KickPlayerReceiver v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TGS.GameProviders.KickPlayerReceiver", Version = "v1" });
            });
            services.AddDbTgsRepository(
                   (sp, dbBuilder) =>
                   {
                       var config = sp.GetRequiredService<IConfiguration>();

                       dbBuilder
                           .UseLazyLoadingProxies();

                       dbBuilder.UseNpgsql(
                           config.GetConnectionString("Tgs"),
                           serverOption =>
                           {
                               serverOption.SetPostgresVersion(12, 5);
                               serverOption.CommandTimeout(180);
                               serverOption.UseTrigrams();
                           });

                       dbBuilder.UseSnakeCaseNamingConvention();
                   });
            services.AddDbBetRepository(
                    (sp, dbBuilder) =>
                    {
                        var config = sp.GetRequiredService<IConfiguration>();

                        dbBuilder
                            .UseLazyLoadingProxies();

                        dbBuilder.UseNpgsql(
                            config.GetConnectionString("Tgs"),
                            serverOption =>
                            {
                                serverOption.SetPostgresVersion(12, 5);
                                serverOption.CommandTimeout(180);
                                serverOption.UseTrigrams();
                            });

                        dbBuilder.UseSnakeCaseNamingConvention();
                    });
            services
               .AddMongoRepository((options, sp) =>
               {
                   var config = sp.GetRequiredService<IConfiguration>();
                   config.GetSection("MongoDBConnectionString").Bind(options);
               });
            services
                .AddHttpClient()
                .AddApiClients();
            services
                .AddCore();
            services.AddRabbitMessageQueue(config =>
            {
                config
                    .ConfigQueueOptions((options, sp) =>
                    {
                        var configuration = sp.GetRequiredService<IConfiguration>();

                        configuration.GetSection("MessageQueue").Bind(options);
                        options.OnSetupQueueAndExchange = channel =>
                        {
                            channel.QueueDeclare(
                                queue: "gp.kickplayer.ky",
                                durable: true,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);

                            channel.QueueBind(
                                "gp.kickplayer.ky",
                                "amq.fanout",
                                $"gp.kickplayer.ky",
                                new Dictionary<string, object>());

                            channel.QueueDeclare(
                               queue: "gp.kickplayer.vr",
                               durable: true,
                               exclusive: false,
                               autoDelete: false,
                               arguments: null);

                            channel.QueueBind(
                                "gp.kickplayer.vr",
                                "amq.fanout",
                                $"gp.kickplayer.vr",
                                new Dictionary<string, object>());

                            channel.QueueDeclare(
                              queue: "gp.kickplayer.wgvia",
                              durable: true,
                              exclusive: false,
                              autoDelete: false,
                              arguments: null);

                            channel.QueueBind(
                                "gp.kickplayer.wgvia",
                                "amq.fanout",
                                $"gp.kickplayer.wgvia",
                                new Dictionary<string, object>());

                            channel.QueueDeclare(
                              queue: "gp.kickplayer.wgfm",
                              durable: true,
                              exclusive: false,
                              autoDelete: false,
                              arguments: null);

                            channel.QueueBind(
                                "gp.kickplayer.wgfm",
                                "amq.fanout",
                                $"gp.kickplayer.wgfm",
                                new Dictionary<string, object>());
                        };
                    })
                   .AddHandler<KYKickPlayerReceiveHandler>("gp.kickplayer.ky", false)
                   .AddHandler<VRKickPlayerReceiveHandler>("gp.kickplayer.vr", false)
                   .AddHandler<WGHorseKickPlayerReceiveHandler>("gp.kickplayer.wgvia", false)
                   .AddHandler<WGSportsKickPlayerReceiveHandler>("gp.kickplayer.wgfm", false);
            });
            services
                .AddKickPlayerHandles();
        }
    }
}
