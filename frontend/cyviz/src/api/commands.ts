import { api } from "./apiClient";
import type {
    CommandRequestDto,
    CommandResponseDto,
    CommandStatusDto,
} from "../types/command";

export const sendCommand = async (
    deviceId: string,
    req: CommandRequestDto
) => {
    const { data } = await api.post<CommandResponseDto>(
        `/devices/${deviceId}/commands`,
        req
    );
    return data;
};

export const getCommandStatus = async (
    deviceId: string,
    commandId: string
) => {
    const { data } = await api.get<CommandStatusDto>(
        `/devices/${deviceId}/commands/${commandId}`
    );
    return data;
};
