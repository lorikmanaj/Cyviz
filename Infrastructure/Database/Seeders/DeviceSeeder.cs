using Cyviz.Core.Application.Interfaces;
using Cyviz.Core.Domain.Entities;
using Cyviz.Core.Domain.Enums;

namespace Cyviz.Infrastructure.Database.Seeders
{
    public interface IDeviceSeeder
    {
        Task SeedDevicesAsync();
    }

    public class DeviceSeeder : IDeviceSeeder
    {
        private readonly IDeviceRepository _repo;

        public DeviceSeeder(IDeviceRepository repo)
        {
            _repo = repo;
        }

        public async Task SeedDevicesAsync()
        {
            var random = new Random();
            var types = Enum.GetValues<DeviceType>();
            var protocols = Enum.GetValues<DeviceProtocol>();

            var devices = Enumerable.Range(1, 20).Select(i => new Device
            {
                Id = $"device-{i:00}",
                Name = $"Device-{i:00}",
                Type = types[random.Next(types.Length)],
                Protocol = protocols[random.Next(protocols.Length)],
                Capabilities = new List<string> { "Ping", "GetStatus", "Reboot" },
                Status = DeviceStatus.Offline,
                LastSeenUtc = DateTime.UtcNow,
                Firmware = "v1.0.0",
                Location = "Lab-A"
            }).ToList();

            await _repo.AddRangeAsync(devices);
        }
    }
}
