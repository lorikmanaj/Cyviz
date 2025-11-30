import type { DeviceListDto } from "../dtos/DeviceListDto";
import type { DeviceDetailDto } from "../types/device";
import { api } from "./axios";

export const getDevices = async (after?: string) => {
    const { data } = await api.get<DeviceListDto[]>(`/devices`, {
        params: {
            top: 20,
            after
        },
    });
    return data;
};

export const getDeviceDetails = async (id: string) => {
    const { data } = await api.get<DeviceDetailDto>(`/devices/${id}`);
    return data;
};