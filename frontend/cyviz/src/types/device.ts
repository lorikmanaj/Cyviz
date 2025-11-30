export interface DeviceListItem {
    id: string;
    name: string;
    status: string;
}

export interface TelemetrySnapshot {
    deviceId: string;
    timestampUtc: string;
    dataJson: string;
}

export interface DeviceDetailDto {
    id: string;
    name: string;
    type: string;
    status: string;
    lastSeenUtc: string;
    firmware: string;
    location: string;
    latestTelemetry?: TelemetrySnapshot;
}

export interface KeysetPage<T> {
    items: T[];
    nextCursor?: string | null;
}
