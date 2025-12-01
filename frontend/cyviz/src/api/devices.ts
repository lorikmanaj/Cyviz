import { api } from "./apiClient";
import type { DeviceListDto, DeviceDetailDto } from "../types/device";
import type { KeysetPageResult } from "../types/pagination/keysetPageResult";

export const getDevices = async (after?: string) => {
    const params: { top: number; after?: string } = { top: 20 };
    if (after) params.after = after;

    const { data } = await api.get<KeysetPageResult<DeviceListDto>>("/devices", {
        params,
    });

    return data; // { items, nextCursor }
};

export const getDeviceDetails = async (id: string) => {
    const { data } = await api.get<DeviceDetailDto>(`/devices/${id}`);
    return data;
};
