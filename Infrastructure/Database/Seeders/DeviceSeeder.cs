namespace Cyviz.Infrastructure.Database.Seeders
{
    public interface IDeviceSeeder
    {
        Task SeedDevicesAsync();
    }

    public class DeviceSeeder : IDeviceSeeder
    {
        public Task SeedDevicesAsync()
        {
            throw new NotImplementedException();
        }
    }
}
