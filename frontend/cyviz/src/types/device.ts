export interface DeviceListDto {
    id: string;
    name: string;
    type: DeviceType;
    status: DeviceStatus;
    location: string;
    lastSeenUtc: string;
}

export interface DeviceDetailDto {
    id: string;
    name: string;
    type: DeviceType;
    protocol: DeviceProtocol;
    capabilities: string[];
    status: DeviceStatus;
    firmware: string;
    location: string;
    lastSeenUtc: string;
    latestTelemetry?: DeviceSnapshotDto | null;
}

export interface DeviceSnapshotDto {
    deviceId: string;
    timestampUtc: string;
    dataJson: string; // JSON string containing telemetry
}

export type DeviceType = "Display" | "Codec" | "Switcher" | "Sensor";

export type DeviceProtocol = "TcpLine" | "HttpJson" | "EdgeSignalR";

export type DeviceStatus = "Online" | "Offline";

export interface DeviceStatusChangedEvent {
  deviceId: string;
  status: "Online" | "Offline";
}
