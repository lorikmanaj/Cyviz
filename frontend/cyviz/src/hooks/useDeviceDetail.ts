import { useQuery } from "@tanstack/react-query";
import { getDeviceDetails } from "../api/devices";

export const useDeviceDetail = (id: string) => {
    return useQuery({
        queryKey: ["device", id],
        queryFn: () => getDeviceDetails(id),
    });
};