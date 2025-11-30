using Cyviz.Core.Domain.Entities;
using Cyviz.Core.Domain.Entities.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Cyviz.Infrastructure.Database
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<Device> Devices { get; set; }
        public DbSet<DeviceTelemetry> DeviceTelemetry { get; set; }
        public DbSet<DeviceCommand> DeviceCommands { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DeviceConfiguration).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DeviceTelemetryConfiguration).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DeviceCommandConfiguration).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}
