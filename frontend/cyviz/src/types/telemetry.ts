export interface TelemetryDto {
    timestampUtc: string;
    dataJson: string;
}

// Parsed telemetry structure if needed
export interface TelemetryPayload {
    cpu?: number;
    mem?: number;
    temp?: number;
}

export interface TelemetryUpdateEvent {
    deviceId: string;
    timestampUtc: string;
    payload: string; // MUST parse JSON
}