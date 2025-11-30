using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace Cyviz.Core.Domain.Entities.Configurations
{
    public class DeviceConfiguration : IEntityTypeConfiguration<Device>
    {
        public void Configure(EntityTypeBuilder<Device> builder)
        {
            builder.HasKey(d => d.Id);

            builder.Property(d => d.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(d => d.Type)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(d => d.Protocol)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(d => d.Status)
                .HasConversion<string>()
                .IsRequired();

            // Capabilities → JSON conversion
            builder.Property(d => d.Capabilities)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null))
                .HasColumnType("TEXT");

            // Concurrency token (ETag)
            builder.Property(d => d.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            // Indexes
            builder.HasIndex(d => d.Status);
            builder.HasIndex(d => d.Type);
            builder.HasIndex(d => d.Name);
        }
    }
}
