using Cyviz.Core.Application.DTOs.DeviceSnapshot;

namespace Cyviz.Core.Application.Interfaces
{
    public interface IDeviceSnapshotCache
    {
        DeviceSnapshotDto? GetSnapshot(string deviceId);
        void SetSnapshot(DeviceSnapshotDto snapshot);
    }
}
