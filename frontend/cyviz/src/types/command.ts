export interface CommandRequestDto {
    command: string;
    idempotencyKey: string;
}

export interface CommandResponseDto {
    commandId: string;
    status: CommandStatus;
}

export interface CommandStatusDto {
    commandId: string;
    status: CommandStatus;
    createdUtc: string;
    completedUtc?: string | null;
}

export type CommandStatus = "Pending" | "Completed" | "Failed";

//SignalR specific
export interface CommandSignalREvent {
    deviceId: string;
    commandId: string;
    status: CommandStatus; // "Completed" | "Failed"
}