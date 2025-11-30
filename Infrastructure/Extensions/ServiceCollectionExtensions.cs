using Cyviz.Core.Application.Interfaces;
using Cyviz.Core.Application.Mappings;
using Cyviz.Core.Application.Repositories;
using Cyviz.Core.Application.Services;
using Cyviz.Infrastructure.Caching;
using Cyviz.Infrastructure.Database;
using Cyviz.Infrastructure.Middlewares;
using Cyviz.Infrastructure.ProtocolAdapters_PLACEHOLDERS_;
using Cyviz.Infrastructure.Repositories.Entities;
using Cyviz.Infrastructure.Services;
using Cyviz.SignalR.Pipelines;
using Cyviz.SignalR.Workers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

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

            // Add Mappers
            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<DeviceMappingProfile>();
                cfg.AddProfile<CommandMappingProfile>();
                cfg.AddProfile<TelemetryMappingProfile>();
            });

            // Add Repositories
            services.AddScoped<IDeviceRepository, DeviceRepository>();
            services.AddScoped<IDeviceTelemetryRepository, DeviceTelemetryRepository>();
            services.AddScoped<IDeviceCommandRepository, DeviceCommandRepository>();

            // Add Services
            services.AddScoped<IDeviceService, DeviceService>();
            services.AddScoped<IDeviceTelemetryService, DeviceTelemetryService>();
            services.AddScoped<IDeviceCommandService, DeviceCommandService>();

            // Add Caching
            services.AddMemoryCache();
            services.AddSingleton<IDeviceSnapshotCache, DeviceSnapshotCache>();

            //// Add (signalR) Pipeline + Workers
            ///Signleton for CommandPipeline so it can be shared
            services.AddSingleton<ICommandPipeline, CommandChannelPipeline>();
            //Web HostedService so it can run in background
            services.AddHostedService<WorkerManager>();

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
