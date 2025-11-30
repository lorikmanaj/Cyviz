import { useInfiniteQuery } from "@tanstack/react-query";
import { getDevices } from "../api/devices";
import type { DeviceListDto } from "../dtos/DeviceListDto";
import type { KeysetPage } from "../types/device";

export const useDevices = () =>
    useInfiniteQuery<
        KeysetPage<DeviceListDto>,   // lastPage type
        Error,
        KeysetPage<DeviceListDto>    // page type
    >({
        queryKey: ["devices"],
        queryFn: ({ pageParam }) => getDevices(pageParam ?? null),
        initialPageParam: null as string | null,
        getNextPageParam: (last) => last.nextCursor ?? null,
    });
