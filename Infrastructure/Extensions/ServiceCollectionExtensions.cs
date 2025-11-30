using Cyviz.Core.Application.Interfaces;
using Cyviz.Infrastructure.Caching;
using Cyviz.Infrastructure.Database;
using Cyviz.Infrastructure.Middlewares;
using Cyviz.Infrastructure.ProtocolAdapters_PLACEHOLDERS_;
using Cyviz.Infrastructure.Repositories.Entities;
using Cyviz.SignalR.Pipelines;
using Cyviz.SignalR.Workers;

namespace Cyviz.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            // SQLite DB
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlite(config.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("CyvizAPI"));
            });

            // Add Repositories
            services.AddScoped<IDeviceRepository, DeviceRepository>();
            services.AddScoped<ITelemetryRepository, TelemetryRepository>();
            services.AddScoped<IDeviceCommandRepository, DeviceCommandRepository>();

            // Add Caching
            services.AddMemoryCache();
            services.AddSingleton<IDeviceSnapshotCache, DeviceSnapshotCache>();

            // Add Pipeline + Workers
            services.AddSingleton<ICommandPipeline, CommandChannelPipeline>();
            services.AddSingleton<IWorkerManager, WorkerManager>();

            // Protocol Adapters (placeholders)
            //services.AddScoped<IDeviceProtocolAdapter, EdgeSignalRAdapter>();

            // Middlewares
            services.AddScoped<RequestLoggingMiddleware>();
            services.AddScoped<ApiKeyMiddleware>();
            services.AddScoped<ExceptionMiddleware>();

            return services;
        }
    }
}
