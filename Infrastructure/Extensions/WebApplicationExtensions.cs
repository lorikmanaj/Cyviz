using Cyviz.Core.Application.Interfaces;
using Cyviz.Infrastructure.Database;

namespace Cyviz.Infrastructure.Extensions
{
    public static class WebApplicationExtensions
    {
        public static WebApplication RunMigrations(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var repo = scope.ServiceProvider.GetRequiredService<IDeviceRepository>();

            db.Database.Migrate();
            db.Database.EnsureCreated();

            // Seed devices on first run
            if (!repo.AnyDevices())
            {
                var seeder = scope.ServiceProvider.GetRequiredService<IDeviceSeeder>();
                seeder.SeedDevicesAsync().Wait();
            }

            return app;
        }

        public static void StartSignalRWorkers(this WebApplication app)
        {
            var workerManager = app.Services.GetRequiredService<IWorkerManager>();
            workerManager.StartWorkers();
        }
    }
}
