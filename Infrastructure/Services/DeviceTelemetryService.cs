using AutoMapper;
using Cyviz.Core.Application.DTOs.DeviceSnapshot;
using Cyviz.Core.Application.DTOs.Telemetry;
using Cyviz.Core.Application.Interfaces;
using Cyviz.Core.Application.Repositories;
using Cyviz.Core.Application.Services;
using Cyviz.Core.Domain.Entities;
using Cyviz.Core.Domain.Enums;

namespace Cyviz.Infrastructure.Services
{
    public class DeviceTelemetryService(
        IDeviceRepository deviceRepo,
        IDeviceTelemetryRepository telemetryRepo,
        IDeviceSnapshotCache snapshotCache,
        IMapper mapper) : IDeviceTelemetryService
    {
        private readonly IDeviceRepository _deviceRepo = deviceRepo;
        private readonly IDeviceTelemetryRepository _telemetryRepo = telemetryRepo;
        private readonly IDeviceSnapshotCache _snapshotCache = snapshotCache;
        private readonly IMapper _mapper = mapper;

        public async Task AddTelemetryAsync(string deviceId, string dataJson, DateTime timestampUtc)
        {
            var device = await _deviceRepo.GetByIdAsync(deviceId)
                ?? throw new Exception("Device not found");

            // Update device "last seen" + bring Online
            device.LastSeenUtc = timestampUtc;
            device.Status = DeviceStatus.Online;
            await _deviceRepo.UpdateAsync(device);

            var telemetry = new DeviceTelemetry
            {
                DeviceId = deviceId,
                TimestampUtc = timestampUtc,
                DataJson = dataJson
            };

            await _telemetryRepo.AddTelemetryAsync(telemetry);

            // Enforce <= 50 entries
            await _telemetryRepo.TrimHistoryAsync(deviceId);

            await _telemetryRepo.SaveChangesAsync();

            // Update in-memory snapshot
            var snapshot = new DeviceSnapshotDto
            {
                DeviceId = deviceId,
                TimestampUtc = timestampUtc,
                DataJson = dataJson
            };

            _snapshotCache.SetLatestSnapshot(snapshot);
        }

        public async Task<IReadOnlyList<TelemetryDto>> GetRecentTelemetryAsync(string deviceId, int limit)
        {
            var entries = await _telemetryRepo.GetRecentAsync(deviceId, limit);
            return _mapper.Map<IReadOnlyList<TelemetryDto>>(entries);
        }

        public DeviceSnapshotDto? GetSnapshot(string deviceId)
        {
            return _snapshotCache.GetLatestSnapshot(deviceId);
        }
    }
}
