using Cyviz.Core.Application.DTOs.DeviceSnapshot;

namespace Cyviz.Core.Application.Interfaces
{
    public interface IDeviceSnapshotCache
    {
        DeviceSnapshotDto? GetLatestSnapshot(string deviceId);
        void SetLatestSnapshot(DeviceSnapshotDto snapshot);
    }
}
