using Cyviz.Core.Application.DTOs.DeviceSnapshot;
using Cyviz.Core.Application.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Cyviz.Infrastructure.Caching
{
    public class DeviceSnapshotCache(IMemoryCache cache) : IDeviceSnapshotCache
    {
        private readonly IMemoryCache _cache = cache;

        public DeviceSnapshotDto? GetSnapshot(string deviceId)
        {
            _cache.TryGetValue(deviceId, out DeviceSnapshotDto? snapshot);
            return snapshot;
        }

        public void SetSnapshot(DeviceSnapshotDto snapshot)
        {
            var cacheOptions = new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(5)
            };

            _cache.Set(snapshot.DeviceId, snapshot, cacheOptions);
        }
    }
}
