import { useQuery } from "@tanstack/react-query";
import { getDeviceDetails } from "../api/devices";
import type { DeviceDetailDto } from "../types/device";

export const useDeviceDetails = (id?: string) =>
    useQuery<DeviceDetailDto, Error>({
        queryKey: ["device", id],
        queryFn: () => getDeviceDetails(id!),
        enabled: !!id,
    });
