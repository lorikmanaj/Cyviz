using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cyviz.Core.Domain.Entities.Configurations
{
    public class DeviceTelemetryConfiguration : IEntityTypeConfiguration<DeviceTelemetry>
    {
        public void Configure(EntityTypeBuilder<DeviceTelemetry> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.DataJson)
                   .IsRequired();

            builder.HasIndex(t => t.DeviceId);
            builder.HasIndex(t => t.TimestampUtc);

            builder.HasOne<Device>()
                   .WithMany()
                   .HasForeignKey(t => t.DeviceId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
