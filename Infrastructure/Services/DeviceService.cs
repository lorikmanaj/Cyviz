using AutoMapper;
using Cyviz.Core.Application.DTOs.Device;
using Cyviz.Core.Application.DTOs.DeviceSnapshot;
using Cyviz.Core.Application.Exceptions;
using Cyviz.Core.Application.Interfaces;
using Cyviz.Core.Application.Models.Pagination;
using Cyviz.Core.Application.Repositories;
using Cyviz.Core.Application.Services;
using Cyviz.Core.Domain.Entities;
using Cyviz.Core.Domain.Enums;

namespace Cyviz.Infrastructure.Services
{
    public class DeviceService(
        IDeviceRepository repo,
        IDeviceTelemetryRepository telemetryRepo,
        IDeviceSnapshotCache snapshotCache,
        IMapper mapper) : IDeviceService
    {
        private readonly IDeviceRepository _repo = repo;
        private readonly IDeviceTelemetryRepository _telemetryRepo = telemetryRepo;
        private readonly IDeviceSnapshotCache _snapshotCache = snapshotCache;
        private readonly IMapper _mapper = mapper;


        public async Task<KeysetPageResult<DeviceListDto>> GetDevicesAsync(string? after, int pageSize)
        {
            var page = await _repo.GetDevicesKeysetAsync(after, pageSize);

            return new KeysetPageResult<DeviceListDto>
            {
                Items = _mapper.Map<IEnumerable<DeviceListDto>>(page.Items),
                NextCursor = page.NextCursor
            };
        }

        public async Task<DeviceDetailDto> GetDeviceByIdAsync(string id)
        {
            var device = await _repo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Device not found");

            var dto = _mapper.Map<DeviceDetailDto>(device);

            // latest telemetry snapshot (in-memory cache)
            var snapshot = _snapshotCache.GetLatestSnapshot(id);
            dto.LatestTelemetry = snapshot;

            return dto;
        }

        public Task UpdateDeviceAsync(string id, DeviceUpdateDto dto)
        {
            // DO NOT allow updates without ETag -> Specification
            throw new PreconditionFailedException(
                "Missing If-Match ETag header. Concurrency update requires ETag.");
        }

        public async Task UpdateDeviceAsync(string id, DeviceUpdateDto dto, byte[] ifMatchRowVersion)
        {
            var device = await _repo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Device not found");

            // ETag concurrency check
            if (!device.RowVersion.SequenceEqual(ifMatchRowVersion))
                throw new PreconditionFailedException("ETag mismatch – device was modified.");

            // map allowed fields
            device.Location = dto.Location;
            device.Name = dto.Name;
            device.Firmware = dto.Firmware;

            await _repo.UpdateAsync(device);
        }

        public async Task AppendTelemetryAsync(string deviceId, DeviceTelemetry telemetry)
        {
            await _telemetryRepo.AddAsync(telemetry);

            // keep only last 50
            await _telemetryRepo.TrimHistoryAsync(deviceId, 50);

            // update snapshot
            _snapshotCache.SetLatestSnapshot(new DeviceSnapshotDto
            {
                DeviceId = deviceId,
                TimestampUtc = telemetry.TimestampUtc,
                DataJson = telemetry.DataJson
            });
        }

        public async Task MarkDeviceOnlineAsync(string deviceId)
        {
            var device = await _repo.GetByIdAsync(deviceId)
                ?? throw new KeyNotFoundException($"Device {deviceId} not found");

            var now = DateTime.UtcNow;

            // Update device
            device.LastSeenUtc = now;

            if (device.Status != DeviceStatus.Online)
                device.Status = DeviceStatus.Online;

            await _repo.UpdateAsync(device);

            // Update snapshot cache
            _snapshotCache.SetLatestSnapshot(new DeviceSnapshotDto
            {
                DeviceId = deviceId,
                TimestampUtc = now,
                DataJson = null // heartbeat has no telemetry payload
            });
        }
    }
}
