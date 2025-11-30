using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cyviz.Core.Domain.Entities.Configurations
{
    public class DeviceCommandConfiguration : IEntityTypeConfiguration<DeviceCommand>
    {
        public void Configure(EntityTypeBuilder<DeviceCommand> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Command)
                .IsRequired();

            builder.Property(c => c.IdempotencyKey)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.Status)
                .HasConversion<string>()
                .IsRequired();

            builder.HasIndex(c => new { c.DeviceId, c.IdempotencyKey }).IsUnique();

            builder.HasOne<Device>()
                   .WithMany()
                   .HasForeignKey(c => c.DeviceId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
